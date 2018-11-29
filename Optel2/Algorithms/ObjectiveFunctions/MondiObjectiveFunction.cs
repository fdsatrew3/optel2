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
            ExecutionTimeAndCost executionTimeAndCost = new ExecutionTimeAndCost();
            DateTime executionStart = DateTime.Now, executionEnd = DateTime.Now;
            decimal workTime = 0,
                    totalWorkTime = 0,
                    workCost = 0,
                    totalRetargetingTime = 0,
                    retargetingTime = 0;

            FilmRecipe filmRecipe;

            filmRecipes = db.FilmRecipes.ToList();

            // Есть заказы на линии.
            if (Orders.Count > 0)
            {
                // Самый первый заказ.
                totalWorkTime += Convert.ToDecimal(DateTimeToDouble(Line.ChangeOfThicknessTime));
                filmRecipe = GetFilmRecipe(Orders[0]);
                // Начнёт выполняться не раньше, чем линия перенастроится.
                Orders[0].PlanedStartDate = executionStart.AddSeconds(Convert.ToDouble(totalWorkTime));

                //Extruder.StartupDelay + Extruder.ChangeofThickness(Order1, Order2) + Extruder.WidthAdj(Order1, Order2) + CalcRetargettingTIme(Order1, Order2) + (Order2.Rolls * Order2.width) / FilmRecipe.GetProductionSpeedByOrderCode(Order2)) 

                Orders[0].PlanedEndDate = Orders[0].PlanedStartDate.AddSeconds(DateTimeToDouble(Line.ChangeOfThicknessTime) + DateTimeToDouble(Line.WidthAdjustmentTime) + Convert.ToDouble(Orders[0].Rolls * Orders[0].Width / filmRecipe.ProductionSpeed) * 60);
                executionStart = Orders[0].PlanedStartDate;

                // Прочие заказы.
                if (Orders.Count > 1)
                {
                    for (int i = 1; i < Orders.Count; i++)
                    {
                        workTime = 0;

                        // Время перенастройки с предыдущего заказа на текущий.
                        retargetingTime = Convert.ToDecimal(RetargetingTimeCalculator(Line, Orders[i - 1], Orders[i]));

                        // i-тый заказ начнёт выполняться не раньше, чем перенастроится линия (отсчёт с первого 
                        // заказа: totalWorkTime хранит как полезную работу, так и предыдущие перенастройки).
                        double d = Convert.ToDouble(totalWorkTime + retargetingTime);
                        Orders[i].PlanedStartDate = Orders[0].PlanedStartDate.AddHours(d);
                        workTime += Convert.ToDecimal(Orders[i].Rolls * Orders[i].Width / filmRecipe.ProductionSpeed) * 60 + Convert.ToDecimal(DateTimeToDouble(Line.WidthAdjustmentTime));
                        // Отсчёт завершения работы i-того заказа - от времени начала его завершения.
                        Orders[i].PlanedEndDate = Orders[i].PlanedStartDate.AddSeconds(Convert.ToDouble(workTime));

                        // Аккумулируем общее время перенастроек.
                        totalRetargetingTime += retargetingTime;

                        // Аккумулируем общее время работы.
                        totalWorkTime += workTime + retargetingTime;
                    }
                }

                executionEnd = Orders[Orders.Count - 1].PlanedEndDate;
            }

            executionTimeAndCost = new ExecutionTimeAndCost()
            {
                ExecutionTime = totalWorkTime,
                RetargetingTime = totalRetargetingTime,
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
            return Convert.ToDouble(date.Year * 31556926 + date.Month * 2629743.83 + date.Hour * 3600 + date.Minute * 60 + date.Second);
        }

        protected FilmRecipe GetFilmRecipe(Order order)
        {
            try
            {
                return filmRecipes.Where(film => film.Article == $"{order.Product}{order.Width}").First();
            }
            catch (Exception)
            { return filmRecipes[0]; }
        }

        protected double RetargetingTimeCalculator(Extruder line, Order previousOrder, Order newOrder)
        {
            double retargetingTime = 0;

            var Extruders = db.Extruders.Include(e => e.ExtruderCalibrationChange).Include(e => e.ExtruderCoolingLipChange).Include(e => e.ExtruderNozzleChange).Include(e => e.ExtruderRecipeChange).ToList();

            //var key = newOrder.FilmTypeVector.FPserieCode + previousOrder.FilmTypeVector.FPserieCode;

            if ($"{newOrder.Product}{newOrder.Width}" != $"{previousOrder.Product}{previousOrder.Width}")
            {
                FilmRecipe newRecipe = GetFilmRecipe(newOrder);

                ExtruderRecipeChange extruderRecipeChange;
                ExtruderCoolingLipChange extruderCoolingLipChange;
                ExtruderCalibrationChange extruderCalibrationChange;
                ExtruderNozzleChange extruderNozzleChange;

                try
                {
                    extruderRecipeChange = line.ExtruderRecipeChange.Where(recipe => (recipe.From == $"{GetFilmRecipe(previousOrder).Article}") && (recipe.On == $"{GetFilmRecipe(newOrder).Article}")).First();
                }
                catch (Exception)
                { extruderRecipeChange = line.ExtruderRecipeChange[0]; }
                try
                {
                    extruderCoolingLipChange = line.ExtruderCoolingLipChange.Where(coolLip => coolLip.CoolingLip == newRecipe.CoolingLip).First();
                }
                catch (Exception)
                { extruderCoolingLipChange = line.ExtruderCoolingLipChange[0]; }
                try
                {
                    extruderCalibrationChange = line.ExtruderCalibrationChange.Where(calibrate => calibrate.Calibration == newRecipe.CalibrationDiameter).First();
                }
                catch (Exception)
                { extruderCalibrationChange = line.ExtruderCalibrationChange[0]; }
                try
                {
                    extruderNozzleChange = line.ExtruderNozzleChange.Where(nozzle => nozzle.Nozzle == newRecipe.NozzleInsert).First();
                }
                catch (Exception)
                { extruderNozzleChange = line.ExtruderNozzleChange[0]; }

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
