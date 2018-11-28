using Optel2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Optel2.Controllers
{
    public class PlanningController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: Planning/Config
        public async Task<ActionResult> Config()
        {
            PlanningModel planningModel = new PlanningModel();
            planningModel.Orders = await db.Orders.ToListAsync();
            return View(planningModel);
        }

        // POST: Planning/Config
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Config([Bind(Include = "PlannedStartDate, PlannedEndDate, Orders, Extruders, Criterion, Function, MaxPopulation, MaxSelection, MutationPropability, PercentOfMutableGens, CrossoverPropability, NumberOfGAiterations")] PlanningModel planningConfig)
        {
            if (ModelState.IsValid)
            {
                Order order = planningConfig.Orders.First(o => o.OrderNumber.Equals("101576"));
                return RedirectToAction("Result");
            }
            PlanningModel planningModel = new PlanningModel();
            planningModel.Orders = await db.Orders.ToListAsync();
            return View(planningModel);
        }

        public ActionResult Result()
        {
            return View();
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