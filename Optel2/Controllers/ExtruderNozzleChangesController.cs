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
    public class ExtruderNozzleChangesController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: ExtruderNozzleChanges
        public async Task<ActionResult> Index()
        {
            return View(await db.ExtruderNozzleChanges.ToListAsync());
        }

        // GET: ExtruderNozzleChanges/Details/5
        public async Task<ActionResult> Details(Guid? id)
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
            return View(extruderNozzleChange);
        }

        // GET: ExtruderNozzleChanges/Create
        public ActionResult Create()
        {
            return View();
        }

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

            return View(extruderNozzleChange);
        }

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
            return View(extruderNozzleChange);
        }

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
            return View(extruderNozzleChange);
        }

        // GET: ExtruderNozzleChanges/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
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
            return View(extruderNozzleChange);
        }

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
