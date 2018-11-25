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
    [Authorize(Roles = "Admin")]
    public class ExtrudersController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: Extruders
        public async Task<ActionResult> Index()
        {
            return View(await db.Extruders.ToListAsync());
        }

        // GET: Extruders/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Extruder extruder = await db.Extruders.FindAsync(id);
            if (extruder == null)
            {
                return HttpNotFound();
            }
            return View(extruder);
        }

        // GET: Extruders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Extruders/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,WidthMin,WidthMax,ThicknessMin,ThicknessMax,ProductionSpeedMin,ProductionSpeedMax,DiameterMin,DiameterMax,WeightMin,WeightMax,LenghtMin,LenghtMax,WidthAdjustmentTime,ChangeOfThicknessTime,StartupDelay,MachineHourCost,WidthAdjustmentConsumption,ChangeOfThicknessTimeConsumption")] Extruder extruder)
        {
            if (ModelState.IsValid)
            {
                extruder.Id = Guid.NewGuid();
                db.Extruders.Add(extruder);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(extruder);
        }

        // GET: Extruders/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Extruder extruder = await db.Extruders.FindAsync(id);
            if (extruder == null)
            {
                return HttpNotFound();
            }
            return View(extruder);
        }

        // POST: Extruders/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,WidthMin,WidthMax,ThicknessMin,ThicknessMax,ProductionSpeedMin,ProductionSpeedMax,DiameterMin,DiameterMax,WeightMin,WeightMax,LenghtMin,LenghtMax,WidthAdjustmentTime,ChangeOfThicknessTime,StartupDelay,MachineHourCost,WidthAdjustmentConsumption,ChangeOfThicknessTimeConsumption")] Extruder extruder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(extruder).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(extruder);
        }

        // GET: Extruders/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Extruder extruder = await db.Extruders.FindAsync(id);
            if (extruder == null)
            {
                return HttpNotFound();
            }
            return View(extruder);
        }

        // POST: Extruders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Extruder extruder = await db.Extruders.FindAsync(id);
            db.Extruders.Remove(extruder);
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
