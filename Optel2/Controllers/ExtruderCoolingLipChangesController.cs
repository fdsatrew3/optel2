using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Optel2.Models;
using X.PagedList;
using System.Web.Configuration;

namespace Optel2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExtruderCoolingLipChangesController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: ExtruderCoolingLipChanges
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageContent = await db.ExtruderCoolingLipChanges.Include(i => i.Extruder).OrderBy(i => i.Extruder.Name).ToPagedListAsync(pageNumber, Convert.ToInt32(WebConfigurationManager.AppSettings["ElementsPerIndexPage"]));
            ViewBag.PageContent = pageContent;
            return View();
        }

        // GET: ExtruderCoolingLipChanges/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderCoolingLipChange extruderCoolingLipChange = await db.ExtruderCoolingLipChanges.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (extruderCoolingLipChange == null)
            {
                return HttpNotFound();
            }
            return View(extruderCoolingLipChange);
        }

        // GET: ExtruderCoolingLipChanges/Create
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

        // POST: ExtruderCoolingLipChanges/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CoolingLip,Duration,Consumption,ExtruderId")] ExtruderCoolingLipChange extruderCoolingLipChange)
        {
            if (ModelState.IsValid)
            {
                extruderCoolingLipChange.Id = Guid.NewGuid();
                db.ExtruderCoolingLipChanges.Add(extruderCoolingLipChange);
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
            return View(extruderCoolingLipChange);
        }

        // GET: ExtruderCoolingLipChanges/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderCoolingLipChange extruderCoolingLipChange = await db.ExtruderCoolingLipChanges.FindAsync(id);
            if (extruderCoolingLipChange == null)
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
            return View(extruderCoolingLipChange);
        }

        // POST: ExtruderCoolingLipChanges/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CoolingLip,Duration,Consumption,ExtruderId")] ExtruderCoolingLipChange extruderCoolingLipChange)
        {
            if (ModelState.IsValid)
            {
                db.Entry(extruderCoolingLipChange).State = EntityState.Modified;
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
            return View(extruderCoolingLipChange);
        }

        // GET: ExtruderCoolingLipChanges/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderCoolingLipChange extruderCoolingLipChange = await db.ExtruderCoolingLipChanges.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (extruderCoolingLipChange == null)
            {
                return HttpNotFound();
            }
            return View(extruderCoolingLipChange);
        }

        // POST: ExtruderCoolingLipChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            ExtruderCoolingLipChange extruderCoolingLipChange = await db.ExtruderCoolingLipChanges.FindAsync(id);
            db.ExtruderCoolingLipChanges.Remove(extruderCoolingLipChange);
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
