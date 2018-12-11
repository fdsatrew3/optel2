﻿using Algorithms;
using Algorithms.BruteForce;
using Algorithms.ObjectiveFunctions;
using GenetycAlgorithm;
using Optel2.DestoyThisPls;
using Optel2.Models;
using Optel2.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Algorithms.ProductionPlan;

namespace Optel2.Controllers
{
    [AuthorizeRoles]
    public class PlanningController : Controller
    {
        private OptelContext db = new OptelContext();

        // GET: Planning/Config
        public async Task<ActionResult> Config()
        {
            PlanningModel planningModel = new PlanningModel();
            planningModel.Orders = await db.Orders.OrderBy(o => o.OrderNumber).ToListAsync();
            planningModel.Extruders = await db.Extruders.Include(e => e.ExtruderCalibrationChange)
                .Include(e => e.ExtruderCoolingLipChange)
                .Include(e => e.ExtruderNozzleChange)
                .Include(e => e.ExtruderRecipeChange).ToListAsync();
            List<SelectListItem> criterionDropDownList = new List<SelectListItem>();
            criterionDropDownList.Add(new SelectListItem() { Text = OptimizationCriterion.Cost.ToString(), Value = OptimizationCriterion.Cost.ToString() });
            criterionDropDownList.Add(new SelectListItem() { Text = OptimizationCriterion.Time.ToString(), Value = OptimizationCriterion.Time.ToString() });
            ViewBag.Criterions = criterionDropDownList;
            List<SelectListItem> algorithmDropDownList = new List<SelectListItem>();
            algorithmDropDownList.Add(new SelectListItem() { Text = "Genetic", Value = PlanningModel.PlanningAlgorithm.Genetic.ToString() });
            algorithmDropDownList.Add(new SelectListItem() { Text = "Brute force", Value = PlanningModel.PlanningAlgorithm.BruteForce.ToString() });
            ViewBag.Algorithms = algorithmDropDownList;
            planningModel.NumberOfGAiterations = 100;
            planningModel.maxPopulation = 10;
            planningModel.maxSelection = 10;
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
            if (planningConfig.PlannedStartDate > planningConfig.PlannedEndDate)
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
            planningModel.maxPopulation = 10;
            planningModel.maxSelection = 10;
            return View(planningModel);
        }

        public string GenerateJSON(ProductionPlan plan)
        {
            string json = "";
            int id = 1;
            for (int i = 0; i < plan.OrdersToLineConformity.Count; i++)
            {
                json += "[{\r\n";
                json += "\"id\": \"" + id + "\",\r\n";
                json += "\"name\": \"" + plan.OrdersToLineConformity[i].Line.Name + "\",\r\n";
                json += "\"periods\": [\r\n";
                for (int j = 0; j < plan.OrdersToLineConformity[i].Orders.Count; j++)
                {
                    json += "{\r\n\"id\": \"" + id + "_" + j + "_" + plan.OrdersToLineConformity[i].Line.Name + "\",\r\n";
                    json += "\"stroke\": \"3 black\",\r\n";
                    json += "\"start\": Date.UTC(" + plan.OrdersToLineConformity[i].Orders[j].PlanedStartDate.ToString("yyyy, M, d, H, m, s") + "),\r\n";
                    json += "\"end\": Date.UTC(" + plan.OrdersToLineConformity[i].Orders[j].PlanedEndDate.ToString("yyyy, M, d, H, m, s") + "),\r\n},\r\n";
                }
                json += "],\r\n},";
                id++;
            }
            json = json.Substring(0, json.Length - 1);
            json += "\r\n]";
            return json;
        }

        public async Task<ActionResult> Result(PlanningModel planningConfig)
        {
            planningConfig.Extruders = (List<Extruder>)TempData["Extruders"];
            planningConfig.Orders = (List<Order>)TempData["Orders"];
            if (planningConfig.Orders == null || planningConfig.Extruders == null)
            {
                return RedirectToAction("Config");
            }
            TempData["Extruders"] = planningConfig.Extruders;
            TempData["Orders"] = planningConfig.Orders;
            MyLittleKostyl.startDate = planningConfig.PlannedStartDate;
            MyLittleKostyl.endDate = planningConfig.PlannedEndDate;
            ProductionPlan result = new ProductionPlan();
            /*result.OrdersToLineConformity = new List<OrdersOnExtruderLine>() { new OrdersOnExtruderLine() };
            result.OrdersToLineConformity[0].Line = planningConfig.Extruders[0];
            result.OrdersToLineConformity[0].Orders = new List<Order> { planningConfig.Orders[0], planningConfig.Orders[1] };
            result.OrdersToLineConformity[0].Orders[0].PlanedStartDate = DateTime.Now;
            result.OrdersToLineConformity[0].Orders[0].PlanedEndDate = DateTime.Now.AddDays(4);
            result.OrdersToLineConformity[0].Orders[1].PlanedStartDate = result.OrdersToLineConformity[0].Orders[0].PlanedEndDate.AddDays(1);
            result.OrdersToLineConformity[0].Orders[1].PlanedEndDate = result.OrdersToLineConformity[0].Orders[1].PlanedStartDate.AddHours(4);
            */
            switch (planningConfig.SelectedAlgorithm)
            {
                case PlanningModel.PlanningAlgorithm.BruteForce:
                    result = await Task.Run<ProductionPlan>(
                        async () => await new BruteForceAlgorithm().Start(planningConfig.Extruders,
                        planningConfig.Orders,
                        new List<SliceLine>(),
                        new Costs(),
                        planningConfig.Criterion,
                        new MondiObjectiveFunction()));
                    break;
                case PlanningModel.PlanningAlgorithm.Genetic:
                    result = await Task.Run<ProductionPlan>(
                        async () => await new GeneticAlgorithm().Start(planningConfig.Extruders,
                        planningConfig.Orders,
                        new List<SliceLine>(),
                        new Costs(),
                        planningConfig.Criterion,
                        new MondiObjectiveFunction(),
                        planningConfig.maxPopulation,
                        planningConfig.NumberOfGAiterations,
                        planningConfig.maxSelection));
                    break;
            }
            ViewBag.JsonString = GenerateJSON(result);
            ViewBag.Criteria = planningConfig.Criterion == OptimizationCriterion.Cost ? "Cost" : "Time";
            decimal temp = result.GetWorkSpending(new Costs(), planningConfig.Criterion, new MondiObjectiveFunction());
            ViewBag.Result = Math.Round(temp, 2);
            ViewBag.Result1 = DateTime.MinValue.AddSeconds(Convert.ToDouble(temp)).ToString("d H:mm:ss");
            return View(planningConfig);
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