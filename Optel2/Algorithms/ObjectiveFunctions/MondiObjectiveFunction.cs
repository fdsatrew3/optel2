using Optel2.DestoyThisPls;
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
        public override ExecutionTimeAndCost GetExecutionTimeAndCost(Costs costs, Extruder Line, List<Order> Orders)
        {
            ExecutionTimeAndCost executionTimeAndCost = new ExecutionTimeAndCost();
            DateTime executionStart = MyLittleKostyl.startDate, executionEnd = MyLittleKostyl.endDate;
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
                Orders[0].PlanedStartDate = executionStart;
                Orders[0].PlanedEndDate = executionStart.AddSeconds(workTime);
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

                        // Аккумулируем общее время перенастроек.
                        totalRetargetingTime += retargetingTime;

                        // Аккумулируем общее время работы.
                        totalWorkTime += workTime + retargetingTime;
                    }
                }

                executionEnd = Orders[Orders.Count - 1].PlanedEndDate;
            }
            workCost = Convert.ToDecimal((totalWorkTime / 3600)) * Line.MachineHourCost;
            executionTimeAndCost = new ExecutionTimeAndCost()
            {
                ExecutionTime = Convert.ToDecimal(totalWorkTime),
                RetargetingTime = Convert.ToDecimal(totalRetargetingTime),
                ExecutionCost = workCost,
                ExecutionStart = executionStart,
                ExecutionEnd = executionEnd
            };

            return executionTimeAndCost;
        }

        private double CalcOrderExecutionTime(Order order)
        {
            if(order.PredefinedTime != 0)
            {
                return order.PredefinedTime;
            } else
            {
                return Convert.ToDouble(order.QuanityInRunningMeter / (order.FilmRecipe.ProductionSpeed / 60));
            }
        }

        protected double DateTimeToDouble(DateTime date)
        {
            return Convert.ToDouble(date.Hour * 3600 + date.Minute * 60 + date.Second);
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

            if (!$"{newOrder.Product}{newOrder.Width}".Equals($"{previousOrder.Product}{previousOrder.Width}"))
            {
                FilmRecipe newRecipe = newOrder.FilmRecipe;

                ExtruderRecipeChange extruderRecipeChange = null;
                ExtruderCoolingLipChange extruderCoolingLipChange = null;
                ExtruderCalibrationChange extruderCalibrationChange = null;
                ExtruderNozzleChange extruderNozzleChange = null;
                for (int i = 0; i < line.ExtruderRecipeChange.Count; i++)
                {
                    if (line.ExtruderRecipeChange[i].From.Equals($"{previousOrder.FilmRecipe.Article}") && line.ExtruderRecipeChange[i].On.Equals($"{newOrder.FilmRecipe.Article}"))
                    {
                        extruderRecipeChange = line.ExtruderRecipeChange[i];
                        break;
                    }
                }
                if (extruderRecipeChange == null)
                {
                    extruderRecipeChange = line.ExtruderRecipeChange[0];
                }
                for (int i = 0; i < line.ExtruderCoolingLipChange.Count; i++)
                {
                    if (line.ExtruderCoolingLipChange[i].CoolingLip.Equals(newRecipe.CoolingLip))
                    {
                        extruderCoolingLipChange = line.ExtruderCoolingLipChange[i];
                        break;
                    }
                }
                if (extruderCoolingLipChange == null)
                {
                    extruderCoolingLipChange = line.ExtruderCoolingLipChange[0];
                }
                for (int i = 0; i < line.ExtruderCalibrationChange.Count; i++)
                {
                    if (line.ExtruderCalibrationChange[i].Calibration.Equals(newRecipe.CalibrationDiameter))
                    {
                        extruderCalibrationChange = line.ExtruderCalibrationChange[i];
                        break;
                    }
                }
                if (extruderCalibrationChange == null)
                {
                    extruderCalibrationChange = line.ExtruderCalibrationChange[0];
                }
                for (int i = 0; i < line.ExtruderNozzleChange.Count; i++)
                {
                    if (line.ExtruderNozzleChange[i].Nozzle.Equals(newRecipe.NozzleInsert))
                    {
                        extruderNozzleChange = line.ExtruderNozzleChange[i];
                        break;
                    }
                }
                if (extruderNozzleChange == null)
                {
                    extruderNozzleChange = line.ExtruderNozzleChange[0];
                }
                retargetingTime += DateTimeToDouble(extruderRecipeChange.Duration)
                    + DateTimeToDouble(extruderCoolingLipChange.Duration)
                    + DateTimeToDouble(extruderCalibrationChange.Duration)
                    + DateTimeToDouble(extruderNozzleChange.Duration);
            }

            // Изменение ширины плёнки.
            if (previousOrder.Width != newOrder.Width)
            {
                retargetingTime += DateTimeToDouble(line.WidthAdjustmentTime);
            }
            if(previousOrder.FilmRecipe.Thickness != newOrder.FilmRecipe.Thickness)
            {
                retargetingTime += DateTimeToDouble(line.ChangeOfThicknessTime);
            }
            return retargetingTime;
        }
    }
}
