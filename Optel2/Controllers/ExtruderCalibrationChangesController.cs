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
    public class ExtruderCalibrationChangesController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: ExtruderCalibrationChanges
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageContent = await db.ExtruderCalibrationChanges.Include(i => i.Extruder).OrderBy(i => i.Extruder.Name).ToPagedListAsync(pageNumber, Convert.ToInt32(WebConfigurationManager.AppSettings["ElementsPerIndexPage"]));
            ViewBag.PageContent = pageContent;
            return View();
        }

        // GET: ExtruderCalibrationChanges/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderCalibrationChange extruderCalibrationChange = await db.ExtruderCalibrationChanges.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (extruderCalibrationChange == null)
            {
                return HttpNotFound();
            }
            return View(extruderCalibrationChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: ExtruderCalibrationChanges/Create
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
        // POST: ExtruderCalibrationChanges/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Calibration,Duration,Consumption,ExtruderId")] ExtruderCalibrationChange extruderCalibrationChange)
        {
            if (ModelState.IsValid)
            {
                extruderCalibrationChange.Id = Guid.NewGuid();
                db.ExtruderCalibrationChanges.Add(extruderCalibrationChange);
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
            return View(extruderCalibrationChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: ExtruderCalibrationChanges/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderCalibrationChange extruderCalibrationChange = await db.ExtruderCalibrationChanges.FindAsync(id);
            if (extruderCalibrationChange == null)
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
            return View(extruderCalibrationChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: ExtruderCalibrationChanges/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Calibration,Duration,Consumption,ExtruderId")] ExtruderCalibrationChange extruderCalibrationChange)
        {
            if (ModelState.IsValid)
            {
                db.Entry(extruderCalibrationChange).State = EntityState.Modified;
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
            return View(extruderCalibrationChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: ExtruderCalibrationChanges/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderCalibrationChange extruderCalibrationChange = await db.ExtruderCalibrationChanges.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (extruderCalibrationChange == null)
            {
                return HttpNotFound();
            }
            return View(extruderCalibrationChange);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: ExtruderCalibrationChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            ExtruderCalibrationChange extruderCalibrationChange = await db.ExtruderCalibrationChanges.FindAsync(id);
            db.ExtruderCalibrationChanges.Remove(extruderCalibrationChange);
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
