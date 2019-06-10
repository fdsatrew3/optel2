using Algorithms;
using Algorithms.BruteForce;
using Algorithms.ObjectiveFunctions;
using GenetycAlgorithm;
using Newtonsoft.Json.Linq;
using Optel2.Algorithms;
using Optel2.DestoyThisPls;
using Optel2.Models;
using Optel2.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
            planningModel.Orders = await db.Orders.OrderBy(o => o.OrderNumber).Include(o => o.FilmRecipe).ToListAsync();
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
            algorithmDropDownList.Add(new SelectListItem() { Text = "Old plan", Value = PlanningModel.PlanningAlgorithm.OldPlan.ToString() });
            ViewBag.Algorithms = algorithmDropDownList;
            planningModel.NumberOfGAiterations = 7;
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
            List<Order> sortedOrders;
            List<Extruder> sortedExtruders;
            if (planningConfig.SelectedAlgorithm == PlanningModel.PlanningAlgorithm.OldPlan)
            {
                sortedOrders = planningConfig.Orders.ToList();
                sortedExtruders = planningConfig.Extruders.Where(extruder => extruder.Name == "MEX 08").ToList();
            } else
            {
                sortedOrders = planningConfig.Orders.Where(order => order.Selected == true).ToList();
                sortedExtruders = planningConfig.Extruders.Where(extruder => extruder.Selected == true).ToList();
            }
            if (sortedOrders.Count < 2)
            {
                ModelState.AddModelError("", "You must select at least two orders.");
            }

            if (sortedExtruders.Count == 0)
            {
                ModelState.AddModelError("", "You must select at least one extruder.");
            }
            planningConfig.NumberOfGAiterations = 7;
            planningConfig.maxPopulation = 10;
            planningConfig.maxSelection = 10;
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
            planningModel.NumberOfGAiterations = 7;
            planningModel.maxPopulation = 10;
            planningModel.maxSelection = 10;
            return View(planningModel);
        }

        public string GenerateJSON(ProductionPlan plan)
        {
            JObject jsonContainer = new JObject();
            JArray dataContainer = new JArray();
            int id = 1;
            for (int i = 0; i < plan.OrdersToLineConformity.Count; i++)
            {
                JObject line = new JObject();
                JProperty lineID = new JProperty("id", id);
                JProperty lineName = new JProperty("text", plan.OrdersToLineConformity[i].Line.Name);
                JProperty lineOpen = new JProperty("open", true);
                JProperty lineStartDate = new JProperty("start_date", plan.OrdersToLineConformity[i].Orders[0].PlanedStartDate.ToString("dd.MM.yyyy HH:mm:ss"));
                JProperty lineEndDate = new JProperty("end_date", plan.OrdersToLineConformity[i].Orders[plan.OrdersToLineConformity[i].Orders.Count-1].PlanedEndDate.ToString("dd.MM.yyyy HH:mm:ss"));
                line.Add(lineID);
                line.Add(lineName);
                line.Add(lineOpen);
                line.Add(lineStartDate);
                line.Add(lineEndDate);
                dataContainer.Add(line);
                for (int j = 0; j < plan.OrdersToLineConformity[i].Orders.Count; j++)
                {
                    JObject order = new JObject();
                    JProperty orderID = new JProperty("id", plan.OrdersToLineConformity[i].Orders[j].OrderNumber + "_" + i + "_" + j);
                    JProperty orderName = new JProperty("text", plan.OrdersToLineConformity[i].Orders[j].OrderNumber);
                    JProperty orderStartDate = new JProperty("start_date", plan.OrdersToLineConformity[i].Orders[j].PlanedStartDate.ToString("dd.MM.yyyy HH:mm:ss"));
                    JProperty orderEndDate = new JProperty("end_date", plan.OrdersToLineConformity[i].Orders[j].PlanedEndDate.ToString("dd.MM.yyyy HH:mm:ss"));
                    JProperty orderParent = new JProperty("parent", id);
                    order.Add(orderID);
                    order.Add(orderName);
                    order.Add(orderStartDate);
                    order.Add(orderEndDate);
                    order.Add(orderParent);
                    dataContainer.Add(order);
                }
                id++;
            }
            jsonContainer["data"] = dataContainer;
            return jsonContainer.ToString();
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
            planningConfig.maxPopulation = planningConfig.Orders.Count;
            planningConfig.maxSelection = planningConfig.Orders.Count;
            planningConfig.NumberOfGAiterations = planningConfig.Orders.Count / 2;
            switch (planningConfig.SelectedAlgorithm)
             {
                 case PlanningModel.PlanningAlgorithm.BruteForce:
                     var bruteForce = new BruteForceAlgorithm();
                     result = await Task.Run<ProductionPlan>(
                         async () => await bruteForce.Start(planningConfig.Extruders,
                         planningConfig.Orders,
                         new List<SliceLine>(),
                         new Costs(),
                         planningConfig.Criterion,
                         new MondiObjectiveFunction(),
                         planningConfig.TreeRequired));
                     planningConfig.TreeData = bruteForce.DecisionTree;
                     break;
                 case PlanningModel.PlanningAlgorithm.Genetic:
                     var genetic = new GeneticAlgorithm();
                     result = await Task.Run<ProductionPlan>(
                         async () => await genetic.Start(planningConfig.Extruders,
                         planningConfig.Orders,
                         new List<SliceLine>(),
                         new Costs(),
                         planningConfig.Criterion,
                         new MondiObjectiveFunction(),
                         planningConfig.maxPopulation,
                         planningConfig.NumberOfGAiterations,
                         planningConfig.maxSelection,
                         planningConfig.TreeRequired));
                     planningConfig.TreeData = genetic.DecisionTree;
                     break;
                case PlanningModel.PlanningAlgorithm.OldPlan:
                    result = GetProductionPlan(planningConfig.Orders, planningConfig.Extruders, planningConfig.PlannedStartDate);
                    planningConfig.TreeRequired = false;
                    break;
            }

            if (planningConfig.TreeRequired)
            {
                string treeDataJSON = "var treeDataJSON = [];\n";
                for (int i = 0; i < planningConfig.TreeData.Count; i++)
                {
                    if ((i + 1) != planningConfig.TreeData.Count)
                    {
                        planningConfig.TreeData[i].Next = planningConfig.TreeData[i + 1];
                        planningConfig.TreeData[i].Next.Iteration = i + 1;
                    }
                    planningConfig.TreeData[i].Iteration = i;
                    Debug.WriteLine("Iter 1 = " + planningConfig.TreeData[i].Plan.GetWorkSpending(null, OptimizationCriterion.Time, new MondiObjectiveFunction()));
                    treeDataJSON += "treeDataJSON.push(" + GenerateJSON(planningConfig.TreeData[i].Plan) + ");\n";
                }
                ViewBag.DecisionTreeElementsJSON = treeDataJSON;
                ViewBag.DecisionTreeString = GenerateDecisionTreeHTML(planningConfig);
            }
            ViewBag.JsonString = GenerateJSON(result);
            Debug.Print(GenerateJSON(result));
            ViewBag.Criteria = planningConfig.Criterion == OptimizationCriterion.Cost ? "Cost" : "Time";
            double requiredTime = Convert.ToDouble(result.GetWorkSpending(null, OptimizationCriterion.Time, new MondiObjectiveFunction()));
            if (requiredTime > (planningConfig.PlannedEndDate - planningConfig.PlannedStartDate).TotalSeconds)
            {
                ViewBag.Error = true;
            }
            else
            {
                ViewBag.Error = false;
                if (planningConfig.Criterion == OptimizationCriterion.Cost)
                {
                    ViewBag.Result = Math.Round(result.GetWorkSpending(null, OptimizationCriterion.Cost, new MondiObjectiveFunction()), 2);
                }
                else
                {
                    ViewBag.Result = GetTotalTime(requiredTime);
                }
            }
            return View(planningConfig);
        }

        public string GenerateDecisionTreeHTML(PlanningModel planningConfig)
        {
            List<Decision> treeData = planningConfig.TreeData;
            string result = "";
            int countOfStarts = 0;
            for (int i = 0; i < treeData.Count; i++)
            {
                if (treeData[i].Iteration == 0)
                {
                    countOfStarts++;
                }
            }
            Debug.WriteIf(countOfStarts == 0, "DecisionTree, starts count = 0. Wtf?");
            string startClass = "entry sole";
            if (countOfStarts > 1)
            {
                startClass = "entry";
            }
            result += "<div class=\"" + startClass + " \"><span class=\"label toggleable\" iter=\"" + treeData[0].Iteration + "\">";
            if (planningConfig.Criterion == OptimizationCriterion.Cost)
            {
                result += Math.Round(treeData[0].FunctionValue, 2) + "$";
            }
            else
            {
                result += GetTotalTime(Convert.ToDouble(treeData[0].FunctionValue));
            }
            result += "</span>";
            result += GenerateDecisionTreeChildHTML(treeData[0].Next, planningConfig.Criterion);
            result += "</div>";
            return result;
        }

        string GenerateDecisionTreeChildHTML(Decision child, OptimizationCriterion criterion)
        {
            string result = "";
            if (child != null && child.Iteration > -1)
            {
                result += "<div class=\"branch\"><div class=\"entry sole\" ><span class=\"label toggleable\" iter=\"" + child.Iteration + "\">";
                if (criterion == OptimizationCriterion.Cost)
                {
                    result += Math.Round(child.FunctionValue, 2) + "$";
                }
                else
                {
                    result += GetTotalTime(Convert.ToDouble(child.FunctionValue));
                }
                result += "</span>";
                result += GenerateDecisionTreeChildHTML(child.Next, criterion);
                result += "</div></div>";
            }
            return result;
        }
        public string GetTotalTime(double seconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            int months = 0;
            int days = ts.Days;
            string result = (months > 0 ? months.ToString() + " month(s), " : "") + (days > 0 ? days.ToString() + " day(s), " : "") + ts.ToString(@"hh\:mm\:ss");
            return result;
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