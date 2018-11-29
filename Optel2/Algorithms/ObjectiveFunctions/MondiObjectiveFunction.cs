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
        OptelContext db = new OptelContext();
        List<FilmRecipe> filmRecipes;

        public override ExecutionTimeAndCost GetExecutionTimeAndCost(Costs costs, Extruder Line, List<Order> Orders)
        {
            double d = DateTimeToDouble(Line.ChangeOfThicknessTime);
            ExecutionTimeAndCost executionTimeAndCost = new ExecutionTimeAndCost();
            DateTime executionStart = MyLittleKostyl.startDate, executionEnd = MyLittleKostyl.endDate;
            double workTime = 0,
                    totalWorkTime = 0,
                    totalRetargetingTime = 0,
                    retargetingTime = 0;
            decimal workCost = 0;
            FilmRecipe filmRecipe;

            filmRecipes = db.FilmRecipes.ToList();

            // Есть заказы на линии.
            if (Orders.Count > 0)
            {
                // Самый первый заказ.
                totalWorkTime += DateTimeToDouble(Line.ChangeOfThicknessTime);
                totalWorkTime += DateTimeToDouble(Line.WidthAdjustmentTime);
                totalWorkTime += DateTimeToDouble(Line.StartupDelay);
                filmRecipe = GetFilmRecipe(Orders[0]);
                // Начнёт выполняться не раньше, чем линия перенастроится.
                Orders[0].PlanedStartDate = executionStart;
                Orders[0].PlanedEndDate = executionStart.AddSeconds(totalWorkTime);
                workTime = Convert.ToDouble(Orders[0].Rolls * Orders[0].Width / (filmRecipe.ProductionSpeed / 60)) + DateTimeToDouble(Line.WidthAdjustmentTime);
                Orders[0].PlanedEndDate.AddSeconds(workTime);
                workCost += 3.23m;
                //Extruder.StartupDelay + Extruder.ChangeofThickness(Order1, Order2) + Extruder.WidthAdj(Order1, Order2) + CalcRetargettingTIme(Order1, Order2) + (Order2.Rolls * Order2.width) / FilmRecipe.GetProductionSpeedByOrderCode(Order2)) 

                //Orders[0].PlanedEndDate = Orders[0].PlanedStartDate.AddSeconds(DateTimeToDouble(Line.ChangeOfThicknessTime) + DateTimeToDouble(Line.WidthAdjustmentTime) + Convert.ToDecimal(Orders[0].Rolls * Orders[0].Width / filmRecipe.ProductionSpeed) * 60);

                // Прочие заказы.
                if (Orders.Count > 1)
                {
                    for (int i = 1; i < Orders.Count; i++)
                    {
                        workTime = 0;

                        // Время перенастройки с предыдущего заказа на текущий.
                        retargetingTime = RetargetingTimeCalculator(Line, Orders[i - 1], Orders[i]);

                        // i-тый заказ начнёт выполняться не раньше, чем перенастроится линия (отсчёт с первого 
                        // заказа: totalWorkTime хранит как полезную работу, так и предыдущие перенастройки).
                        Orders[i].PlanedStartDate = Orders[i - 1].PlanedEndDate;
                        workTime += Convert.ToDouble(Orders[i].Rolls * Orders[i].Width / (filmRecipe.ProductionSpeed / 60)) + DateTimeToDouble(Line.WidthAdjustmentTime);
                        // Отсчёт завершения работы i-того заказа - от времени начала его завершения.
                        Orders[i].PlanedEndDate = Orders[i].PlanedStartDate.AddSeconds(retargetingTime + workTime);

                        // Аккумулируем общее время перенастроек.
                        totalRetargetingTime += retargetingTime;

                        // Аккумулируем общее время работы.
                        totalWorkTime += workTime + retargetingTime;
                        workCost += 3.23m;
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

        protected override float pRetargetingTimeCalculator(Extruder line, Order previousOrder, Order newOrder)
        {
            float retargetingTime = 0;
            return retargetingTime;
        }

        protected double DateTimeToDouble(DateTime date)
        {
            return Convert.ToDouble(date.Hour * 3600 + date.Minute * 60 + date.Second);
        }

        protected FilmRecipe GetFilmRecipe(Order order)
        {
            for (int i = 0; i < filmRecipes.Count; i++)
            {
                if (filmRecipes[i].Article.Equals($"{order.Product}{order.Width}"))
                {
                    return filmRecipes[i];
                }
            }
            return filmRecipes[0];
        }

        protected double RetargetingTimeCalculator(Extruder line, Order previousOrder, Order newOrder)
        {
            double retargetingTime = 0;

            var Extruders = db.Extruders.Include(e => e.ExtruderCalibrationChange).Include(e => e.ExtruderCoolingLipChange).Include(e => e.ExtruderNozzleChange).Include(e => e.ExtruderRecipeChange).ToList();

            //var key = newOrder.FilmTypeVector.FPserieCode + previousOrder.FilmTypeVector.FPserieCode;

            if (!$"{newOrder.Product}{newOrder.Width}".Equals($"{previousOrder.Product}{previousOrder.Width}"))
            {
                FilmRecipe newRecipe = GetFilmRecipe(newOrder);

                ExtruderRecipeChange extruderRecipeChange = null;
                ExtruderCoolingLipChange extruderCoolingLipChange = null;
                ExtruderCalibrationChange extruderCalibrationChange = null;
                ExtruderNozzleChange extruderNozzleChange = null;
                for (int i = 0; i < line.ExtruderRecipeChange.Count; i++)
                {
                    if (line.ExtruderRecipeChange[i].From.Equals($"{GetFilmRecipe(previousOrder).Article}") && line.ExtruderRecipeChange[i].On.Equals($"{GetFilmRecipe(newOrder).Article}"))
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
                retargetingTime += DateTimeToDouble(line.ChangeOfThicknessTime);
            }
            else
            {
                retargetingTime += 0.33f;
            }

            return retargetingTime;
        }
    }

    public struct ParceRecipe
    {
        public string type;
        public decimal thickness;
        public decimal width;
    }
}
