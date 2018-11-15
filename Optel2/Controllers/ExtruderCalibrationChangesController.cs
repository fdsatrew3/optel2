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

namespace Optel2.Controllers
{
    public class ExtruderCalibrationChangesController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: ExtruderCalibrationChanges
        public async Task<ActionResult> Index()
        {
            return View(await db.ExtruderCalibrationChanges.ToListAsync());
        }

        // GET: ExtruderCalibrationChanges/Details/5
        public async Task<ActionResult> Details(Guid? id)
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
            return View(extruderCalibrationChange);
        }

        // GET: ExtruderCalibrationChanges/Create
        public ActionResult Create()
        {
            return View();
        }

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

            return View(extruderCalibrationChange);
        }

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
            return View(extruderCalibrationChange);
        }

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
            return View(extruderCalibrationChange);
        }

        // GET: ExtruderCalibrationChanges/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
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
            return View(extruderCalibrationChange);
        }

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
