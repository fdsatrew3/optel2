using Optel2.Models;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using X.PagedList;
using System.Linq;
using System.Web.Configuration;
using Optel2.Utils;
using System.Collections.Generic;

namespace Optel2.Controllers
{
    [AuthorizeRoles]
    public class OrdersController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: Orders
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageContent = await db.Orders.OrderBy(i => i.OrderNumber).Include(i => i.FilmRecipe).ToPagedListAsync(pageNumber, Convert.ToInt32(WebConfigurationManager.AppSettings["ElementsPerIndexPage"]));
            ViewBag.PageContent = pageContent;
            return View();
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.Include(e => e.FilmRecipe).FirstOrDefaultAsync(e => e.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: Orders/Create
        public async Task<ActionResult> Create()
        {
            List<FilmRecipe> filmRecipes = await db.FilmRecipes.OrderBy(f => f.Article).ToListAsync();
            List<SelectListItem> filmRecipesDropDownList = new List<SelectListItem>();
            foreach (FilmRecipe recipe in filmRecipes)
            {
                filmRecipesDropDownList.Add(new SelectListItem() { Text = recipe.Article.ToString(), Value = recipe.Id.ToString() });
            }
            ViewBag.FilmRecipes = filmRecipesDropDownList;
            return View();
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: Orders/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderNumber,Product,Width,SetupTime,ProductionTime,ProductionInterruptionTime,TotalTime,FinishedGoods,Granules,Waste,QuanityInRunningMeter,RollWeightNet,Rolls,FilmRecipeId")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid();
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.Include(e => e.FilmRecipe).FirstOrDefaultAsync(e => e.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            List<FilmRecipe> filmRecipes = await db.FilmRecipes.OrderBy(f => f.Article).ToListAsync();
            List<SelectListItem> filmRecipesDropDownList = new List<SelectListItem>();
            foreach (FilmRecipe recipe in filmRecipes)
            {
                filmRecipesDropDownList.Add(new SelectListItem() { Text = recipe.Article.ToString(), Value = recipe.Id.ToString() });
            }
            ViewBag.FilmRecipes = filmRecipesDropDownList;
            return View(order);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: Orders/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNumber,Product,Width,SetupTime,ProductionTime,ProductionInterruptionTime,TotalTime,FinishedGoods,Granules,Waste,QuanityInRunningMeter,RollWeightNet,Rolls,FilmRecipeId")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.Include(e => e.FilmRecipe).FirstOrDefaultAsync(e => e.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
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
