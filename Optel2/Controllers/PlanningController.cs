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

        public string DateTimeToDateUTC(DateTime dt)
        {
            return dt.Year + ", " + (dt.Month - 1) + ", " + dt.Day + ", " + dt.Hour + ", " + dt.Minute + ", " + dt.Second;
        }

        public string GenerateJSON(ProductionPlan plan)
        {
            JArray lineContainer = new JArray();
            int id = 1;
            for (int i = 0; i < plan.OrdersToLineConformity.Count; i++)
            {
                JObject line = new JObject();
                JProperty lineID = new JProperty("id", id);
                JProperty lineName = new JProperty("name", plan.OrdersToLineConformity[i].Line.Name);
                line.Add(lineID);
                line.Add(lineName);
                JArray periods = new JArray();
                for (int j = 0; j < plan.OrdersToLineConformity[i].Orders.Count; j++)
                {
                    JObject period = new JObject();
                    JProperty periodID = new JProperty("id", plan.OrdersToLineConformity[i].Orders[j].OrderNumber + "_" + i + j);
                    JProperty startDate = new JProperty("start", "Date.UTC(" + DateTimeToDateUTC(plan.OrdersToLineConformity[i].Orders[j].PlanedStartDate) + ")");
                    JProperty endDate = new JProperty("end", "Date.UTC(" + DateTimeToDateUTC(plan.OrdersToLineConformity[i].Orders[j].PlanedEndDate) + ")");
                    period.Add(periodID);
                    period.Add(startDate);
                    period.Add(endDate);
                    JProperty stroke, angle, color1, pos1, color2, pos2, color3, pos3, fill;
                    JArray keys = new JArray();
                    JObject key1 = new JObject(), key2 = new JObject(), key3 = new JObject(), fill1 = new JObject();
                    fill = new JProperty("fill", fill1);
                    if (j % 2 == 0)
                    {
                        stroke = new JProperty("stroke", "#B8AA96");
                        angle = new JProperty("angle", "90");
                        color1 = new JProperty("color", "#CFC0A9");
                        pos1 = new JProperty("position", "0");
                        color2 = new JProperty("color", "#E6D5BC");
                        pos2 = new JProperty("position", "0.38");
                        color3 = new JProperty("color", "#E8D9C3");
                        pos3 = new JProperty("position", "1");
                    }
                    else
                    {
                        stroke = new JProperty("stroke", "#9B9292");
                        angle = new JProperty("angle", "90");
                        color1 = new JProperty("color", "#AFA4A4");
                        pos1 = new JProperty("position", "0");
                        color2 = new JProperty("color", "#C2B6B6");
                        pos2 = new JProperty("position", "0.38");
                        color3 = new JProperty("color", "#C8BDBD");
                        pos3 = new JProperty("position", "1");
                    }
                    fill1.Add(angle);
                    key1.Add(color1);
                    key1.Add(pos1);
                    key2.Add(color2);
                    key2.Add(pos2);
                    key3.Add(color3);
                    key3.Add(pos3);
                    keys.Add(key1);
                    keys.Add(key2);
                    keys.Add(key3);
                    fill1["keys"] = keys;
                    period.Add(stroke);
                    period.Add(fill);
                    periods.Add(period);
                }
                line["periods"] = periods;
                lineContainer.Add(line);
                id++;
            }
            //Debug.Write(lineContainer.ToString());
            // Костыль чтобы график заработал. GL HF.
            string json = lineContainer.ToString().Replace("\"start\": \"Date.UTC", "\"start\": Date.UTC").Replace("\"end\": \"Date.UTC", "\"end\": Date.UTC").Replace(")\",", "),");
            /*  string json = "";
              int id = 1;
              for (int i = 0; i < plan.OrdersToLineConformity.Count; i++)
              {
                  json += "[{\r\n";
                  json += "\"id\": \"" + id + "\",\r\n";
                  json += "\"name\": \"" + plan.OrdersToLineConformity[i].Line.Name + "\",\r\n";
                  json += "\"periods\": [\r\n";
                  for (int j = 0; j < plan.OrdersToLineConformity[i].Orders.Count; j++)
                  {
                      json += "{\r\n\"id\": \"" + plan.OrdersToLineConformity[i].Orders[j].OrderNumber + "_" + i + j + "\",\r\n";
                      //json += "\"stroke\": \"3 black\",\r\n";
                      json += "\"start\": Date.UTC(" + DateTimeToDateUTC(plan.OrdersToLineConformity[i].Orders[j].PlanedStartDate) + "),\r\n";
                      json += "\"end\": Date.UTC(" + DateTimeToDateUTC(plan.OrdersToLineConformity[i].Orders[j].PlanedEndDate) + "),\r\n";
                      if (j % 2 == 0)
                      {
                          json += "\"stroke\": \"#B8AA96\",\r\n";
                          json += "\"fill\": {\r\n";
                          json += "\"angle\": 90,\r\n";
                          json += "\"keys\": [\r\n";
                          json += "{\r\n";
                          json += "\"color\": \"#CFC0A9\",\r\n";
                          json += "\"position\": 0\r\n";
                          json += "},\r\n";
                          json += "{\r\n";
                          json += "\"color\": \"#E6D5BC\",\r\n";
                          json += "\"position\": 0.38\r\n";
                          json += "},\r\n";
                          json += "{\r\n";
                          json += "\"color\": \"#E8D9C3\",\r\n";
                          json += "\"position\": 1\r\n";
                          json += "}\r\n";
                          json += "]\r\n";
                          json += "}\r\n},\r\n";
                      }
                      else
                      {
                          json += "\"stroke\": \"#9B9292\",\r\n";
                          json += "\"fill\": {\r\n";
                          json += "\"angle\": 90,\r\n";
                          json += "\"keys\": [\r\n";
                          json += "{\r\n";
                          json += "\"color\": \"#AFA4A4\",\r\n";
                          json += "\"position\": 0\r\n";
                          json += "},\r\n";
                          json += "{\r\n";
                          json += "\"color\": \"#C2B6B6\",\r\n";
                          json += "\"position\": 0.38\r\n";
                          json += "},\r\n";
                          json += "{\r\n";
                          json += "\"color\": \"#C8BDBD\",\r\n";
                          json += "\"position\": 1\r\n";
                          json += "}\r\n";
                          json += "]\r\n";
                          json += "}\r\n},\r\n";
                      }
                  }
                  json += "],\r\n},";
                  id++;
              }
              json = json.Substring(0, json.Length - 1);
              json += "\r\n]"; */
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
            }
            if (planningConfig.TreeRequired)
            {
                string treeDataJSON = "var treeDataJSON = [];\n";
                for (int i = 0; i < planningConfig.TreeData.Count; i++)
                {
                    if ((i + 1) != planningConfig.TreeData.Count)
                    {
                        planningConfig.TreeData[i].Next = planningConfig.TreeData[i + 1];
                    }
                    planningConfig.TreeData[i].Iteration = i;
                    treeDataJSON += "treeDataJSON.push(" + GenerateJSON(planningConfig.TreeData[i].Plan) + ");\n";
                    Debug.WriteLine("Tree progress: " + i + "/" + planningConfig.TreeData.Count.ToString());
                }
                /*
                Debug.WriteLine("Tree count = " + planningConfig.TreeData.Count);
                Debug.WriteLine("Dec with mutation = " + planningConfig.TreeData.Count(x => x.Operation == Decision.OperationType.Mutation));
                Debug.WriteLine("Dec with crossover = " + planningConfig.TreeData.Count(x => x.Operation == Decision.OperationType.Crossover));
                */
                ViewBag.DecisionTreeElementsJSON = treeDataJSON;
                ViewBag.DecisionTreeString = GenerateDecisionTreeHTML(planningConfig);
            }
            ViewBag.JsonString = GenerateJSON(result);
            ViewBag.Criteria = planningConfig.Criterion == OptimizationCriterion.Cost ? "Cost" : "Time";
            ViewBag.Result = Math.Round(result.GetWorkSpending(null, OptimizationCriterion.Cost, new MondiObjectiveFunction()), 2);
            ViewBag.Result1 = GetTotalTime(Convert.ToDouble(result.GetWorkSpending(null, OptimizationCriterion.Time, new MondiObjectiveFunction())));
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
            DateTime dt = DateTime.MinValue.AddSeconds(seconds);
            int months = dt.Month - 1;
            int days = dt.Day - 1;
            string result = (months > 0 ? months.ToString() + " month(s), " : "") + (days > 0 ? days.ToString() + " day(s), " : "") + dt.ToString("H:mm:ss");
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