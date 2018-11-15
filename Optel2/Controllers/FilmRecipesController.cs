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
    public class FilmRecipesController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: FilmRecipes
        public async Task<ActionResult> Index()
        {
            return View(await db.FilmRecipes.ToListAsync());
        }

        // GET: FilmRecipes/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmRecipe filmRecipe = await db.FilmRecipes.FindAsync(id);
            if (filmRecipe == null)
            {
                return HttpNotFound();
            }
            return View(filmRecipe);
        }

        // GET: FilmRecipes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FilmRecipes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ItemNumber,Article,Recipe,Thickness,NozzleInsert,NozzleInsertAlternative,CoolingLip,ProductionSpeed,Output,CalibrationDiameter,ExtruderId")] FilmRecipe filmRecipe)
        {
            if (ModelState.IsValid)
            {
                filmRecipe.Id = Guid.NewGuid();
                db.FilmRecipes.Add(filmRecipe);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(filmRecipe);
        }

        // GET: FilmRecipes/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmRecipe filmRecipe = await db.FilmRecipes.FindAsync(id);
            if (filmRecipe == null)
            {
                return HttpNotFound();
            }
            return View(filmRecipe);
        }

        // POST: FilmRecipes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ItemNumber,Article,Recipe,Thickness,NozzleInsert,NozzleInsertAlternative,CoolingLip,ProductionSpeed,Output,CalibrationDiameter,ExtruderId")] FilmRecipe filmRecipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(filmRecipe).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(filmRecipe);
        }

        // GET: FilmRecipes/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmRecipe filmRecipe = await db.FilmRecipes.FindAsync(id);
            if (filmRecipe == null)
            {
                return HttpNotFound();
            }
            return View(filmRecipe);
        }

        // POST: FilmRecipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            FilmRecipe filmRecipe = await db.FilmRecipes.FindAsync(id);
            db.FilmRecipes.Remove(filmRecipe);
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
