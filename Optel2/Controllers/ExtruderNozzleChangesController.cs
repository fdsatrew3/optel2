using Optel2.Models;
using Optel2.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using X.PagedList;

namespace Optel2.Controllers
{
    [AuthorizeRoles]
    public class ExtruderNozzleChangesController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: ExtruderNozzleChanges
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageContent = await db.ExtruderNozzleChanges.Include(i => i.Extruder).OrderBy(i => i.Extruder.Name).ToPagedListAsync(pageNumber, Convert.ToInt32(WebConfigurationManager.AppSettings["ElementsPerIndexPage"]));
            ViewBag.PageContent = pageContent;
            return View();
        }

        // GET: ExtruderNozzleChanges/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderNozzleChange extruderNozzleChange = await db.ExtruderNozzleChanges.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (extruderNozzleChange == null)
            {
                return HttpNotFound();
            }
            return View(extruderNozzleChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: ExtruderNozzleChanges/Create
        public async Task<ActionResult> Create()
        {
            List<Extruder> extruders = await db.Extruders.ToListAsync();
            List<SelectListItem> extrudersDropDownList = new List<SelectListItem>();
            foreach (Extruder extruder in extruders)
            {
                extrudersDropDownList.Add(new SelectListItem() { Text = extruder.Name.ToString(), Value = extruder.Id.ToString() });
            }
            ViewBag.Extruders = extrudersDropDownList;
            return View();
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: ExtruderNozzleChanges/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nozzle,Duration,Consumption,ExtruderId")] ExtruderNozzleChange extruderNozzleChange)
        {
            if (ModelState.IsValid)
            {
                extruderNozzleChange.Id = Guid.NewGuid();
                db.ExtruderNozzleChanges.Add(extruderNozzleChange);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            List<Extruder> extruders = await db.Extruders.ToListAsync();
            List<SelectListItem> extrudersDropDownList = new List<SelectListItem>();
            foreach (Extruder extruder in extruders)
            {
                extrudersDropDownList.Add(new SelectListItem() { Text = extruder.Name.ToString(), Value = extruder.Id.ToString() });
            }
            ViewBag.Extruders = extrudersDropDownList;
            return View(extruderNozzleChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: ExtruderNozzleChanges/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderNozzleChange extruderNozzleChange = await db.ExtruderNozzleChanges.FindAsync(id);
            if (extruderNozzleChange == null)
            {
                return HttpNotFound();
            }
            List<Extruder> extruders = await db.Extruders.ToListAsync();
            List<SelectListItem> extrudersDropDownList = new List<SelectListItem>();
            foreach (Extruder extruder in extruders)
            {
                extrudersDropDownList.Add(new SelectListItem() { Text = extruder.Name.ToString(), Value = extruder.Id.ToString() });
            }
            ViewBag.Extruders = extrudersDropDownList;
            return View(extruderNozzleChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: ExtruderNozzleChanges/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nozzle,Duration,Consumption,ExtruderId")] ExtruderNozzleChange extruderNozzleChange)
        {
            if (ModelState.IsValid)
            {
                db.Entry(extruderNozzleChange).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            List<Extruder> extruders = await db.Extruders.ToListAsync();
            List<SelectListItem> extrudersDropDownList = new List<SelectListItem>();
            foreach (Extruder extruder in extruders)
            {
                extrudersDropDownList.Add(new SelectListItem() { Text = extruder.Name.ToString(), Value = extruder.Id.ToString() });
            }
            ViewBag.Extruders = extrudersDropDownList;
            return View(extruderNozzleChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: ExtruderNozzleChanges/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderNozzleChange extruderNozzleChange = await db.ExtruderNozzleChanges.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (extruderNozzleChange == null)
            {
                return HttpNotFound();
            }
            return View(extruderNozzleChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: ExtruderNozzleChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            ExtruderNozzleChange extruderNozzleChange = await db.ExtruderNozzleChanges.FindAsync(id);
            db.ExtruderNozzleChanges.Remove(extruderNozzleChange);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
