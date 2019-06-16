using Optel2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.ObjectiveFunctions
{
    public class MondiObjectiveFunction : AObjectiveFunction
    {
        private DateTime _executionStart, _executionEnd;

        public MondiObjectiveFunction(DateTime executionStart, DateTime executionEnd)
        {

        }

        public override ExecutionTimeAndCost GetExecutionTimeAndCost(Costs costs, Extruder Line, List<Order> Orders)
        {
            ExecutionTimeAndCost executionTimeAndCost = new ExecutionTimeAndCost();
            double workTime = 0,
                    totalWorkTime = 0,
                    totalRetargetingTime = 0,
                    retargetingTime = 0;
            decimal workCost = 0;
            // Есть заказы на линии.
            if (Orders.Count > 0)
            {
                // Самый первый заказ.
                workTime = CalcOrderExecutionTime(Orders[0]);
                Orders[0].PlanedStartDate = _executionStart;
                Orders[0].PlanedEndDate = _executionStart.AddSeconds(workTime);
                Orders[0].PredefinedTime = Convert.ToInt32(Math.Round(workTime, 0));
                Orders[0].PredefinedRetargetTime = 0;
                //Orders[0].RetargetLog = "0";
                //Extruder.StartupDelay + Extruder.ChangeofThickness(Order1, Order2) + Extruder.WidthAdj(Order1, Order2) + CalcRetargettingTIme(Order1, Order2) + (Order2.Rolls * Order2.width) / FilmRecipe.GetProductionSpeedByOrderCode(Order2)) 

                //Orders[0].PlanedEndDate = Orders[0].PlanedStartDate.AddSeconds(DateTimeToDouble(Line.ChangeOfThicknessTime) + DateTimeToDouble(Line.WidthAdjustmentTime) + Convert.ToDecimal(Orders[0].Rolls * Orders[0].Width / filmRecipe.ProductionSpeed) * 60);
                totalWorkTime += workTime;
                // Прочие заказы.
                if (Orders.Count > 1)
                {
                    for (int i = 1; i < Orders.Count; i++)
                    {
                        // Время перенастройки с предыдущего заказа на текущий.
                        retargetingTime = RetargetingTimeCalculator(Line, Orders[i - 1], Orders[i]);

                        // i-тый заказ начнёт выполняться не раньше, чем перенастроится линия (отсчёт с первого 
                        // заказа: totalWorkTime хранит как полезную работу, так и предыдущие перенастройки).
                        Orders[i].PlanedStartDate = Orders[i - 1].PlanedEndDate;
                        // новый алгоритм 
                        workTime = CalcOrderExecutionTime(Orders[i]);
                        //старый алгоритм 
                        //workTime += Convert.ToDouble(Orders[i].Rolls * Orders[i].Width / (filmRecipe.ProductionSpeed / 60)) + DateTimeToDouble(Line.WidthAdjustmentTime);
                        // Отсчёт завершения работы i-того заказа - от времени начала его завершения.
                        Orders[i].PlanedEndDate = Orders[i].PlanedStartDate.AddSeconds(retargetingTime + workTime);
                        Orders[i].PredefinedTime = Convert.ToInt32(Math.Round(workTime, 0));
                        Orders[i].PredefinedRetargetTime = Convert.ToInt32(Math.Round(retargetingTime, 0));
                        // Аккумулируем общее время перенастроек.
                        totalRetargetingTime += retargetingTime;

                        // Аккумулируем общее время работы.
                        totalWorkTime += workTime + retargetingTime;
                    }
                }

                _executionEnd = Orders[Orders.Count - 1].PlanedEndDate;
            }
            workCost = Convert.ToDecimal((totalWorkTime / 3600)) * Line.MachineHourCost;
            executionTimeAndCost = new ExecutionTimeAndCost()
            {
                ExecutionTime = Convert.ToDecimal(totalWorkTime),
                RetargetingTime = Convert.ToDecimal(totalRetargetingTime),
                ExecutionCost = workCost,
                ExecutionStart = _executionStart,
                ExecutionEnd = _executionEnd
            };

            return executionTimeAndCost;
        }

        private double CalcOrderExecutionTime(Order order)
        {
            if (order.PredefinedTime != 0)
            {
                return order.PredefinedTime;
            }
            else
            {
                return Convert.ToDouble(order.QuanityInRunningMeter / (order.FilmRecipe.ProductionSpeed / 60));
            }
        }

        protected override float pRetargetingTimeCalculator(Extruder line, Order previousOrder, Order newOrder)
        {
            // ?
            return 0f;
        }

        protected double RetargetingTimeCalculator(Extruder line, Order previousOrder, Order newOrder)
        {
            double retargetingTime = 0;

            //var key = newOrder.FilmTypeVector.FPserieCode + previousOrder.FilmTypeVector.FPserieCode;
            // recipe
            if (!previousOrder.FilmRecipeId.Equals(newOrder.FilmRecipeId))
            {
                //newOrder.RetargetLog = "FilmRecipeChange_";
                ExtruderRecipeChange extruderRecipeChange = null;
                ExtruderCoolingLipChange extruderCoolingLipChange = null;
                ExtruderCalibrationChange extruderCalibrationChange = null;
                ExtruderNozzleChange extruderNozzleChange = null;
                if (!previousOrder.FilmRecipe.Recipe.Equals(newOrder.FilmRecipe.Recipe))
                {
                    for (int i = 0; i < line.ExtruderRecipeChange.Count; i++)
                    {
                        if (line.ExtruderRecipeChange[i].From.Equals(previousOrder.FilmRecipe.Recipe) && line.ExtruderRecipeChange[i].On.Equals(newOrder.FilmRecipe.Recipe))
                        {
                            extruderRecipeChange = line.ExtruderRecipeChange[i];
                            break;
                        }
                    }
                    if (extruderRecipeChange == null)
                    {
                        extruderRecipeChange = line.ExtruderRecipeChange[0];
                    }
                    //newOrder.RetargetLog += "Recipe_" + DateTimeToDouble(extruderRecipeChange.Duration) + "_";
                    retargetingTime += extruderRecipeChange.Duration;
                }
                // cooling lip
                if (!previousOrder.FilmRecipe.CoolingLip.Equals(newOrder.FilmRecipe.CoolingLip))
                {
                    for (int i = 0; i < line.ExtruderCoolingLipChange.Count; i++)
                    {
                        if (line.ExtruderCoolingLipChange[i].CoolingLip.Equals(newOrder.FilmRecipe.CoolingLip))
                        {
                            extruderCoolingLipChange = line.ExtruderCoolingLipChange[i];
                            break;
                        }
                    }
                    if (extruderCoolingLipChange == null)
                    {
                        extruderCoolingLipChange = line.ExtruderCoolingLipChange[0];
                    }
                    //newOrder.RetargetLog += "CoolingLip_" + DateTimeToDouble(extruderCoolingLipChange.Duration) + "_";
                    retargetingTime += extruderCoolingLipChange.Duration;
                }
                // calibration
                if (!previousOrder.FilmRecipe.CalibrationDiameter.Equals(newOrder.FilmRecipe.CalibrationDiameter))
                {
                    for (int i = 0; i < line.ExtruderCalibrationChange.Count; i++)
                    {
                        if (line.ExtruderCalibrationChange[i].Calibration.Equals(newOrder.FilmRecipe.CalibrationDiameter))
                        {
                            extruderCalibrationChange = line.ExtruderCalibrationChange[i];
                            break;
                        }
                    }
                    if (extruderCalibrationChange == null)
                    {
                        extruderCalibrationChange = line.ExtruderCalibrationChange[0];
                    }
                    //newOrder.RetargetLog += "Calibration_" + DateTimeToDouble(extruderCalibrationChange.Duration) + "_";
                    retargetingTime += extruderCalibrationChange.Duration;
                }
                // nozzle
                if (!previousOrder.FilmRecipe.NozzleInsert.Equals(newOrder.FilmRecipe.NozzleInsert))
                {
                    for (int i = 0; i < line.ExtruderNozzleChange.Count; i++)
                    {
                        if (line.ExtruderNozzleChange[i].Nozzle.Equals(newOrder.FilmRecipe.NozzleInsert))
                        {
                            extruderNozzleChange = line.ExtruderNozzleChange[i];
                            break;
                        }
                    }
                    if (extruderNozzleChange == null)
                    {
                        extruderNozzleChange = line.ExtruderNozzleChange[0];
                    }
                    //newOrder.RetargetLog += "Nozzle_" + DateTimeToDouble(extruderNozzleChange.Duration) + "_";
                    retargetingTime += extruderNozzleChange.Duration;
                }
                // Изменение ширины плёнки.
                if (previousOrder.Width != newOrder.Width)
                {
                    retargetingTime += line.WidthAdjustmentTime;
                    //newOrder.RetargetLog += "Width_" + DateTimeToDouble(line.WidthAdjustmentTime) + "_";
                }
                if (previousOrder.FilmRecipe.Thickness != newOrder.FilmRecipe.Thickness)
                {
                    retargetingTime += line.ChangeOfThicknessTime;
                    //newOrder.RetargetLog += "Thickness" + DateTimeToDouble(line.ChangeOfThicknessTime) + "_";
                }
            }
            //newOrder.RetargetLog += retargetingTime;
            return retargetingTime;
        }
    }
}

