using Algorithms;
using Algorithms.BruteForce;
using Algorithms.ObjectiveFunctions;
using GenetycAlgorithm;
using Newtonsoft.Json.Linq;
using Optel2.Algorithms;
using Optel2.Models;
using Optel2.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            }
            else
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
            return View(planningModel);
        }

        public string GenerateProductionPlanJSON(ProductionPlan plan, OptimizationCriterion criterion, AObjectiveFunction objectiveFunction)
        {
            JObject jsonContainer = new JObject();
            JArray dataContainer = new JArray();
            JArray linksContainer = new JArray();
            int id = 0;
            // Фикс порядка заказов в коллекции, без понятия как это работает.
            plan.GetWorkSpending(new Costs(), criterion, objectiveFunction);
            for (int i = 0; i < plan.OrdersToLineConformity.Count; i++)
            {
                JObject line = new JObject();
                JProperty lineID = new JProperty("id", "line_" + id);
                JProperty lineName = new JProperty("text", "Line " + plan.OrdersToLineConformity[i].Line.Name);
                JProperty lineOpen = new JProperty("open", true);
                JProperty lineStartDate = new JProperty("start_date", plan.OrdersToLineConformity[i].Orders[0].PlanedStartDate.ToString("dd.MM.yyyy HH:mm:ss"));
                JProperty lineEndDate = new JProperty("end_date", plan.OrdersToLineConformity[i].Orders[plan.OrdersToLineConformity[i].Orders.Count - 1].PlanedEndDate.ToString("dd.MM.yyyy HH:mm:ss"));
                JProperty lineEditable = new JProperty("editable", false);
                JProperty lineReadonly = new JProperty("readonly", true);
                line.Add(lineID);
                line.Add(lineName);
                line.Add(lineOpen);
                line.Add(lineStartDate);
                line.Add(lineEndDate);
                line.Add(lineEditable);
                line.Add(lineReadonly);
                dataContainer.Add(line);
                for (int j = 0; j < plan.OrdersToLineConformity[i].Orders.Count; j++)
                {
                    JObject order = new JObject();
                    JProperty orderID = new JProperty("id", "order_" + plan.OrdersToLineConformity[i].Orders[j].OrderNumber + "_" + i + "_" + j);
                    JProperty orderName = new JProperty("text", "Order " + plan.OrdersToLineConformity[i].Orders[j].OrderNumber);
                    JProperty orderStartDate = new JProperty("start_date", plan.OrdersToLineConformity[i].Orders[j].PlanedStartDate.ToString("dd.MM.yyyy HH:mm:ss"));
                    JProperty orderEndDate = new JProperty("end_date", plan.OrdersToLineConformity[i].Orders[j].PlanedEndDate.ToString("dd.MM.yyyy HH:mm:ss"));
                    JProperty orderParent = new JProperty("parent", "line_" + id);
                    JProperty orderEditable = new JProperty("editable", false);
                    JProperty orderReadonly = new JProperty("readonly", true);
                    order.Add(orderID);
                    order.Add(orderName);
                    order.Add(orderStartDate);
                    order.Add(orderEndDate);
                    order.Add(orderParent);
                    order.Add(orderEditable);
                    order.Add(orderReadonly);
                    dataContainer.Add(order);
                }
                id++;
            }
            jsonContainer["data"] = dataContainer;
            for (int i = 0; i < plan.OrdersToLineConformity.Count; i++)
            {
                if (plan.OrdersToLineConformity[i].Orders.Count > 0)
                {
                    JObject link = new JObject();
                    JProperty linkID = new JProperty("id", "link_" + i + "_line");
                    JProperty linkSource = new JProperty("source", "line_" + i);
                    JProperty linkTarget = new JProperty("target", "order_" + plan.OrdersToLineConformity[i].Orders[0].OrderNumber + "_" + i + "_0");
                    int start_to_start = 1;
                    JProperty linkType = new JProperty("type", start_to_start);
                    JProperty linkEditable = new JProperty("editable", false);
                    JProperty linkReadonly = new JProperty("readonly", true);
                    link.Add(linkID);
                    link.Add(linkSource);
                    link.Add(linkTarget);
                    link.Add(linkType);
                    link.Add(linkEditable);
                    link.Add(linkReadonly);
                    linksContainer.Add(link);
                    if (plan.OrdersToLineConformity[i].Orders.Count > 1)
                    {
                        link = new JObject();
                        linkID = new JProperty("id", "link_" + i + "_0");
                        linkSource = new JProperty("source", "order_" + plan.OrdersToLineConformity[i].Orders[0].OrderNumber + "_" + i + "_0");
                        linkTarget = new JProperty("target", "order_" + plan.OrdersToLineConformity[i].Orders[1].OrderNumber + "_" + i + "_1");
                        int finish_to_start = 0;
                        linkType = new JProperty("type", finish_to_start);
                        linkEditable = new JProperty("editable", false);
                        linkReadonly = new JProperty("readonly", true);
                        link.Add(linkID);
                        link.Add(linkSource);
                        link.Add(linkTarget);
                        link.Add(linkType);
                        link.Add(linkEditable);
                        link.Add(linkReadonly);
                        linksContainer.Add(link);
                        for (int j = 1; j < plan.OrdersToLineConformity[i].Orders.Count - 1; j++)
                        {
                            link = new JObject();
                            linkID = new JProperty("id", "link_" + i + "_" + j);
                            linkSource = new JProperty("source", "order_" + plan.OrdersToLineConformity[i].Orders[j].OrderNumber + "_" + i + "_" + j);
                            linkTarget = new JProperty("target", "order_" + plan.OrdersToLineConformity[i].Orders[j + 1].OrderNumber + "_" + i + "_" + (j + 1));
                            linkType = new JProperty("type", finish_to_start);
                            linkEditable = new JProperty("editable", false);
                            linkReadonly = new JProperty("readonly", true);
                            link.Add(linkID);
                            link.Add(linkSource);
                            link.Add(linkTarget);
                            link.Add(linkType);
                            link.Add(linkEditable);
                            link.Add(linkReadonly);
                            linksContainer.Add(link);
                        }
                    }
                }
            }
            jsonContainer["links"] = linksContainer;
            return jsonContainer.ToString();
        }

        public string GenerateDecisionTreeJSON(List<Decision> treeData, OptimizationCriterion criterion, AObjectiveFunction objectiveFunction)
        {
            JArray jsonContainer = new JArray();
            int x = 0;
            int marginX = 256;
            for (int i = 0; i < treeData.Count; i++)
            {
                JObject data = new JObject();
                JProperty dataId = new JProperty("id", "data" + i);
                JProperty dataX = new JProperty("x", x);
                JProperty dataY = new JProperty("y", 90);
                string dataName = "Iteration #" + (i + 1) + "\n" + FormatFunctionValue(treeData[i].Plan, Convert.ToDouble(treeData[i].FunctionValue), criterion, objectiveFunction);
                JProperty dataText = new JProperty("text", dataName);
                JProperty dataType = new JProperty("type", "process");
                x += marginX;
                data.Add(dataId);
                data.Add(dataX);
                data.Add(dataY);
                data.Add(dataText);
                data.Add(dataType);
                jsonContainer.Add(data);
                JObject link = new JObject();
                JProperty linkFrom = new JProperty("from", "data" + i);
                JProperty linkTo = new JProperty("to", "data" + (i + 1));
                JProperty linkType = new JProperty("type", "line");
                JProperty linkArrow = new JProperty("forwardArrow", "filled");
                JProperty linkFromSide = new JProperty("fromSide", "right");
                JProperty linkToSide = new JProperty("toSide", "left");
                link.Add(linkFrom);
                link.Add(linkTo);
                link.Add(linkType);
                link.Add(linkArrow);
                link.Add(linkFromSide);
                link.Add(linkToSide);
                jsonContainer.Add(link);
            }
            jsonContainer.RemoveAt(jsonContainer.Count - 1);
            return jsonContainer.ToString();
        }

        public string GenerateDecisionTreeElementsData(List<Decision> treeData, OptimizationCriterion criterion, AObjectiveFunction objectiveFunction)
        {
            string treeDataJSON = String.Empty;
            for (int i = 0; i < treeData.Count; i++)
            {
                treeDataJSON += "treeDataJSON.push(" + GenerateProductionPlanJSON(treeData[i].Plan, criterion, objectiveFunction) + ");\n";
            }
            return treeDataJSON;
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
            ProductionPlan result = new ProductionPlan();
            planningConfig.maxPopulation = planningConfig.Orders.Count;
            planningConfig.maxSelection = planningConfig.Orders.Count;
            planningConfig.NumberOfGAiterations = planningConfig.Orders.Count / 2;
            MondiObjectiveFunction objectiveFunction = new MondiObjectiveFunction(planningConfig.PlannedStartDate, planningConfig.PlannedEndDate);
            switch (planningConfig.SelectedAlgorithm)
            {
                case PlanningModel.PlanningAlgorithm.BruteForce:
                    var bruteForce = new BruteForceAlgorithm();
                    result = await Task.Run(
                        async () => await bruteForce.Start(planningConfig.Extruders,
                        planningConfig.Orders,
                        new List<SliceLine>(),
                        new Costs(),
                        planningConfig.Criterion,
                        objectiveFunction,
                        planningConfig.TreeRequired));
                    planningConfig.TreeData = bruteForce.DecisionTree;
                    break;
                case PlanningModel.PlanningAlgorithm.Genetic:
                    var genetic = new GeneticAlgorithm();
                    result = await Task.Run(
                        async () => await genetic.Start(planningConfig.Extruders,
                        planningConfig.Orders,
                        new List<SliceLine>(),
                        new Costs(),
                        planningConfig.Criterion,
                        objectiveFunction,
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
                ViewBag.DecisionTreeElementsData = GenerateDecisionTreeElementsData(planningConfig.TreeData, planningConfig.Criterion, objectiveFunction);
                ViewBag.DecisionTreeJSON = GenerateDecisionTreeJSON(planningConfig.TreeData, planningConfig.Criterion, objectiveFunction);
            }
            else
            {
                ViewBag.DecisionTreeElementsData = "";
                ViewBag.DecisionTreeJSON = "";
            }
            ViewBag.JsonString = GenerateProductionPlanJSON(result, planningConfig.Criterion, objectiveFunction);
            ViewBag.Criteria = planningConfig.Criterion == OptimizationCriterion.Cost ? "Cost" : "Time";
            double requiredTime = Convert.ToDouble(result.GetWorkSpending(null, OptimizationCriterion.Time, objectiveFunction));
            if (requiredTime > (planningConfig.PlannedEndDate - planningConfig.PlannedStartDate).TotalSeconds)
            {
                ViewBag.Error = true;
            }
            else
            {
                ViewBag.Error = false;
                ViewBag.Result = FormatFunctionValue(result, requiredTime, planningConfig.Criterion, objectiveFunction);
            }
            return View(planningConfig);
        }

        public string FormatFunctionValue(ProductionPlan plan, double seconds, OptimizationCriterion criterion, AObjectiveFunction objectiveFunction)
        {
            string result;
            if (criterion == OptimizationCriterion.Time)
            {
                result = FormatTimeInSeconds(seconds);
            }
            else
            {
                result = Math.Round(plan.GetWorkSpending(null, criterion, objectiveFunction), 2).ToString();
            }
            return result;
        }

        private string FormatTimeInSeconds(double seconds)
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