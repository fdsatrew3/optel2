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
using static Algorithms.ProductionPlan;

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
            planningModel.Extruders = await db.Extruders.Include(e => e.ExtruderCalibrationChange)
                .Include(e => e.ExtruderCoolingLipChange)
                .Include(e => e.ExtruderNozzleChange)
                .Include(e => e.ExtruderRecipeChange).ToListAsync();
            List<SelectListItem> criterionDropDownList = new List<SelectListItem>();
            criterionDropDownList.Add(new SelectListItem() { Text = OptimizationCriterion.Cost.ToString(), Value = OptimizationCriterion.Cost.ToString() });
            criterionDropDownList.Add(new SelectListItem() { Text = OptimizationCriterion.Time.ToString(), Value = OptimizationCriterion.Time.ToString() });
            ViewBag.Criterions = criterionDropDownList;
            List<SelectListItem> algorithmDropDownList = new List<SelectListItem>();
            algorithmDropDownList.Add(new SelectListItem() { Text = PlanningModel.PlanningAlgorithm.Genetic.ToString(), Value = PlanningModel.PlanningAlgorithm.Genetic.ToString() });
            algorithmDropDownList.Add(new SelectListItem() { Text = PlanningModel.PlanningAlgorithm.BruteForce.ToString(), Value = PlanningModel.PlanningAlgorithm.BruteForce.ToString() });
            ViewBag.Algorithms = algorithmDropDownList;
            planningModel.NumberOfGAiterations = 100;
            return View(planningModel);
        }

        // POST: Planning/Config
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Config(PlanningModel planningConfig)
        {
            List<Order> sortedOrders = planningConfig.Orders.Where(order => order.Selected == true).ToList(); 
            List<Extruder> sortedExtruders = planningConfig.Extruders.Where(extruder => extruder.Selected == true).ToList();

            if (sortedOrders.Count < 2)
            {
                ModelState.AddModelError("", "You must select at least two orders.");
            }

            if (sortedExtruders.Count == 0)
            {
                ModelState.AddModelError("", "You must select at least one extruder.");
            }

            if (planningConfig.NumberOfGAiterations == 0)
            {
                ModelState.AddModelError("NumberOfGAiterations", "Count of iterations must be greater than zero.");
            }
            if(planningConfig.PlannedStartDate > planningConfig.PlannedEndDate)
            {
                ModelState.AddModelError("PlannedStartDate", "Planned start date must be earlier than planned end date.");
            }
            if (ModelState.IsValid)
            {
                TempData["Orders"] = sortedOrders;
                TempData["Extruders"] = sortedExtruders;
                return RedirectToAction("Result", planningConfig);
            }
            PlanningModel planningModel = new PlanningModel();
            planningModel.Orders = await db.Orders.ToListAsync();
            planningModel.Extruders = await db.Extruders.Include(e => e.ExtruderCalibrationChange)
            .Include(e => e.ExtruderCoolingLipChange)
            .Include(e => e.ExtruderNozzleChange)
            .Include(e => e.ExtruderRecipeChange).ToListAsync();
            List<SelectListItem> criterionDropDownList = new List<SelectListItem>();
            criterionDropDownList.Add(new SelectListItem() { Text = OptimizationCriterion.Cost.ToString(), Value = OptimizationCriterion.Cost.ToString() });
            criterionDropDownList.Add(new SelectListItem() { Text = OptimizationCriterion.Time.ToString(), Value = OptimizationCriterion.Time.ToString() });
            ViewBag.Criterions = criterionDropDownList;
            List<SelectListItem> algorithmDropDownList = new List<SelectListItem>();
            algorithmDropDownList.Add(new SelectListItem() { Text = PlanningModel.PlanningAlgorithm.Genetic.ToString(), Value = PlanningModel.PlanningAlgorithm.Genetic.ToString() });
            algorithmDropDownList.Add(new SelectListItem() { Text = PlanningModel.PlanningAlgorithm.BruteForce.ToString(), Value = PlanningModel.PlanningAlgorithm.BruteForce.ToString() });
            ViewBag.Algorithms = algorithmDropDownList;
            planningModel.NumberOfGAiterations = 100;
            return View(planningModel);
        }

        public ActionResult Result(PlanningModel planningConfig)
        {
            planningConfig.Extruders = (List<Extruder>)TempData["Extruders"];
            planningConfig.Orders = (List<Order>)TempData["Orders"];
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