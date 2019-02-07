﻿using System;
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
using Optel2.Utils;

namespace Optel2.Controllers
{
    [AuthorizeRoles]
    public class FilmRecipesController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: FilmRecipes
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageContent = await db.FilmRecipes.Include(i => i.Extruder).OrderBy(i => i.Recipe).ToPagedListAsync(pageNumber, Convert.ToInt32(WebConfigurationManager.AppSettings["ElementsPerIndexPage"]));
            ViewBag.PageContent = pageContent;
            return View();
        }

        // GET: FilmRecipes/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmRecipe filmRecipe = await db.FilmRecipes.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (filmRecipe == null)
            {
                return HttpNotFound();
            }
            return View(filmRecipe);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: FilmRecipes/Create
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
            List<Extruder> extruders = await db.Extruders.ToListAsync();
            List<SelectListItem> extrudersDropDownList = new List<SelectListItem>();
            foreach (Extruder extruder in extruders)
            {
                extrudersDropDownList.Add(new SelectListItem() { Text = extruder.Name.ToString(), Value = extruder.Id.ToString() });
            }
            ViewBag.Extruders = extrudersDropDownList;
            return View(filmRecipe);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
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
            List<Extruder> extruders = await db.Extruders.ToListAsync();
            List<SelectListItem> extrudersDropDownList = new List<SelectListItem>();
            foreach (Extruder extruder in extruders)
            {
                extrudersDropDownList.Add(new SelectListItem() { Text = extruder.Name.ToString(), Value = extruder.Id.ToString() });
            }
            ViewBag.Extruders = extrudersDropDownList;
            return View(filmRecipe);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
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
            List<Extruder> extruders = await db.Extruders.ToListAsync();
            List<SelectListItem> extrudersDropDownList = new List<SelectListItem>();
            foreach (Extruder extruder in extruders)
            {
                extrudersDropDownList.Add(new SelectListItem() { Text = extruder.Name.ToString(), Value = extruder.Id.ToString() });
            }
            ViewBag.Extruders = extrudersDropDownList;
            return View(filmRecipe);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: FilmRecipes/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmRecipe filmRecipe = await db.FilmRecipes.Include(e => e.Extruder).FirstOrDefaultAsync(e => e.Id == id);
            if (filmRecipe == null)
            {
                return HttpNotFound();
            }
            return View(filmRecipe);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
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
