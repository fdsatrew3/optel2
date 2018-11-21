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
    public class ExtruderRecipeChangesController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: ExtruderRecipeChanges
        public async Task<ActionResult> Index()
        {
            return View(await db.ExtruderRecipeChanges.Include(e => e.Extruder).ToListAsync());
        }

        // GET: ExtruderRecipeChanges/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderRecipeChange extruderRecipeChange = await db.ExtruderRecipeChanges.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (extruderRecipeChange == null)
            {
                return HttpNotFound();
            }
            return View(extruderRecipeChange);
        }

        // GET: ExtruderRecipeChanges/Create
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

        // POST: ExtruderRecipeChanges/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,From,On,Duration,Consumption,ExtruderId")] ExtruderRecipeChange extruderRecipeChange)
        {
            if (ModelState.IsValid)
            {
                extruderRecipeChange.Id = Guid.NewGuid();
                db.ExtruderRecipeChanges.Add(extruderRecipeChange);
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
            return View(extruderRecipeChange);
        }

        // GET: ExtruderRecipeChanges/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderRecipeChange extruderRecipeChange = await db.ExtruderRecipeChanges.FindAsync(id);
            if (extruderRecipeChange == null)
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
            return View(extruderRecipeChange);
        }

        // POST: ExtruderRecipeChanges/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,From,On,Duration,Consumption,ExtruderId")] ExtruderRecipeChange extruderRecipeChange)
        {
            if (ModelState.IsValid)
            {
                db.Entry(extruderRecipeChange).State = EntityState.Modified;
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
            return View(extruderRecipeChange);
        }

        // GET: ExtruderRecipeChanges/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtruderRecipeChange extruderRecipeChange = await db.ExtruderRecipeChanges.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (extruderRecipeChange == null)
            {
                return HttpNotFound();
            }
            return View(extruderRecipeChange);
        }

        // POST: ExtruderRecipeChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            ExtruderRecipeChange extruderRecipeChange = await db.ExtruderRecipeChanges.FindAsync(id);
            db.ExtruderRecipeChanges.Remove(extruderRecipeChange);
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
