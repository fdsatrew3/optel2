using Optel2.Models;
using System;
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
    [Authorize(Roles = "Admin")]
    public class ExtrudersController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: Extruders
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageContent = await db.Extruders.OrderBy(i => i.Name).ToPagedListAsync(pageNumber, Convert.ToInt32(WebConfigurationManager.AppSettings["ElementsPerIndexPage"]));
            ViewBag.PageContent = pageContent;
            return View();
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
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,WidthMin,WidthMax,ThicknessMin,ThicknessMax,ProductionSpeedMin,ProductionSpeedMax,WidthAdjustmentTime,ChangeOfThicknessTime,StartupDelay,MachineHourCost")] Extruder extruder)
        {
            if (extruder.ThicknessMin >= extruder.ThicknessMax)
            {
                ModelState.AddModelError("ThicknessMin", "Allowed min thickness must be smaller than allowed max thickness.");
            }
            if (extruder.ProductionSpeedMin >= extruder.ProductionSpeedMax)
            {
                ModelState.AddModelError("ProductionSpeedMin", "Allowed min production speed must be smaller than allowed max production speed.");
            }
            if (extruder.WidthMin >= extruder.WidthMax)
            {
                ModelState.AddModelError("WidthMin", "Allowed min width must be smaller than allowed max width.");
            }
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
            if (extruder.ThicknessMin >= extruder.ThicknessMax)
            {
                ModelState.AddModelError("ThicknessMin", "Allowed min thickness must be smaller than allowed max thickness.");
            }
            if (extruder.ProductionSpeedMin >= extruder.ProductionSpeedMax)
            {
                ModelState.AddModelError("ProductionSpeedMin", "Allowed min production speed must be smaller than allowed max production speed.");
            }
            if (extruder.WidthMin >= extruder.WidthMax)
            {
                ModelState.AddModelError("WidthMin", "Allowed min width must be smaller than allowed max width.");
            }
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
