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
    public class FilmRecipeCostsController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: FilmRecipeCosts
        public async Task<ActionResult> Index()
        {
            return View(await db.FilmRecipeCosts.ToListAsync());
        }

        // GET: FilmRecipeCosts/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmRecipeCost filmRecipeCost = await db.FilmRecipeCosts.FindAsync(id);
            if (filmRecipeCost == null)
            {
                return HttpNotFound();
            }
            return View(filmRecipeCost);
        }

        // GET: FilmRecipeCosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FilmRecipeCosts/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Recipe,Thickness,Cost")] FilmRecipeCost filmRecipeCost)
        {
            if (ModelState.IsValid)
            {
                filmRecipeCost.Id = Guid.NewGuid();
                db.FilmRecipeCosts.Add(filmRecipeCost);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(filmRecipeCost);
        }

        // GET: FilmRecipeCosts/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmRecipeCost filmRecipeCost = await db.FilmRecipeCosts.FindAsync(id);
            if (filmRecipeCost == null)
            {
                return HttpNotFound();
            }
            return View(filmRecipeCost);
        }

        // POST: FilmRecipeCosts/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Recipe,Thickness,Cost")] FilmRecipeCost filmRecipeCost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(filmRecipeCost).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(filmRecipeCost);
        }

        // GET: FilmRecipeCosts/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmRecipeCost filmRecipeCost = await db.FilmRecipeCosts.FindAsync(id);
            if (filmRecipeCost == null)
            {
                return HttpNotFound();
            }
            return View(filmRecipeCost);
        }

        // POST: FilmRecipeCosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            FilmRecipeCost filmRecipeCost = await db.FilmRecipeCosts.FindAsync(id);
            db.FilmRecipeCosts.Remove(filmRecipeCost);
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
