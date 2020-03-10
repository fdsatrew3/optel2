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
using X.PagedList;
using System.Web.Configuration;
using Optel2.Utils;

namespace Optel2.Controllers
{
    [AuthorizeRoles]
    public class AlgorithmSettingsController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: AlgorithmSettings
        public async Task<ActionResult> Index()
        {
            AlgorithmSettings settings = await db.AlgorithmSettings.FirstAsync();
            return RedirectToAction("Edit/"+settings.Id.ToString());
        }

        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // GET: AlgorithmSettings/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlgorithmSettings settings = await db.AlgorithmSettings.FindAsync(id);
            if (settings == null)
            {
                return HttpNotFound();
            }
            return View(settings);
        }

        [AuthorizeRoles(Utils.User.Roles.Admin)]
        // POST: AlgorithmSettings/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MaxIterations,NumberOfGAiterations,maxPopulation,maxSelection,mutationPropability,percentOfMutableGens,crossoverPropability")] AlgorithmSettings settings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(settings).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(settings);
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
