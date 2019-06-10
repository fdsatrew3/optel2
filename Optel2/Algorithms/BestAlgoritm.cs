using Algorithms;
using Algorithms.ObjectiveFunctions;
using Optel2.Algorithms;
using Optel2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algorithms.ProductionPlan;

namespace Optel2.Algorithms
{
    public class BestAlgoritm : IDisposable
    {
        private OptelContext db;

        private List<ExtruderRecipeChange> extruderRecipeChange;
        private List<ExtruderCalibrationChange> extruderCalibrationChanges;
        private List<ExtruderCoolingLipChange> extruderCoolingLipChanges;
        private List<ExtruderNozzleChange> extruderNozzleChanges;

        public void Dispose()
        {
            db.Dispose();
        }

        public BestAlgoritm()
        {
            db = new OptelContext();

            extruderRecipeChange = db.ExtruderRecipeChanges.ToList();
            extruderCalibrationChanges = db.ExtruderCalibrationChanges.ToList();
            extruderCoolingLipChanges = db.ExtruderCoolingLipChanges.ToList();
            extruderNozzleChanges = db.ExtruderNozzleChanges.ToList();
        }

        public ProductionPlan Start(List<Extruder> extruderLine, List<Order> ordersToExecute, List<SliceLine> slinesBundle)
        {
            //создаем план
            ProductionPlan productionPlan = new ProductionPlan();
            productionPlan.OrdersToLineConformity = new List<OrdersOnExtruderLine>();

            ordersToExecute = ordersToExecute.OrderByDescending(order => order.ProductionTime).ToList();

            for (int i = 0; i < extruderLine.Count; i++)
            {
                List<Order> orders = new List<Order>();

                for (int j = i; j < ordersToExecute.Count; j += extruderLine.Count)
                {
                    orders.Add(ordersToExecute[j]);
                }

                if (ordersToExecute.Count % extruderLine.Count != 0 && ordersToExecute.Count % extruderLine.Count <= i)
                {
                    int index = ordersToExecute.Count / extruderLine.Count;

                    orders.Add(ordersToExecute[index + i]);
                }

                productionPlan.OrdersToLineConformity.Add(BestProdactionPlan(extruderLine[i], orders, slinesBundle));
            }

            return productionPlan;
        }

            /*
            public ProductionPlan Start(List<Extruder> extruderLine, List<Order> ordersToExecute, List<SliceLine> slinesBundle)
            {
                // работаем только для одной линии (например, первой)
                Extruder extruderLines = extruderLine.First();

                //берем все перенастройки
                List<ExtruderRecipeChange> extruderRecipeChange = db.ExtruderRecipeChanges.ToList();
                List<ExtruderCalibrationChange> extruderCalibrationChanges = db.ExtruderCalibrationChanges.ToList();
                List<ExtruderCoolingLipChange> extruderCoolingLipChanges = db.ExtruderCoolingLipChanges.ToList();
                List<ExtruderNozzleChange> extruderNozzleChanges = db.ExtruderNozzleChanges.ToList();

                //создаем план
                ProductionPlan productionPlan = new ProductionPlan();
                productionPlan.OrdersToLineConformity = new List<OrdersOnExtruderLine>();
                productionPlan.OrdersToLineConformity.Add(new OrdersOnExtruderLine() { Line = extruderLines });
                productionPlan.OrdersToLineConformity.First().Orders = new List<Order>();

                extruderRecipeChange = extruderRecipeChange.OrderByDescending(change => change.Duration).ToList();

                if (extruderRecipeChange.Count == 0)
                    throw new Exception("Нет перенастроек");

                // для начала смотрим все перенастройки в поисках заказов, которые могли бы выполняться
                for (int i = 0; i < extruderRecipeChange.Count; i++)
                {
                    // если в списке заказов есть хотя бы один заказ по такому рецепту
                    if (ordersToExecute.Where(order => order.FilmRecipe.Recipe.Equals(extruderRecipeChange[i].On)).Count() > 0)
                    {
                        // выбираем все заказы по такому рецепту
                        List<Order> orders = ordersToExecute.Where(or => or.FilmRecipe.Recipe.Equals(extruderRecipeChange[i].On)).OrderBy(order => order.FilmRecipe.NozzleInsert).ThenBy(order => order.FilmRecipe.CalibrationDiameter).ThenBy(order => order.FilmRecipe.CoolingLip).ToList();

                        // и вставляем их в план
                        for (int j = 0; j < orders.Count; j++)
                        {
                            productionPlan.OrdersToLineConformity.First().Orders.Add(orders[j]);
                        }

                        break; // заказы выбраны - сваливаем
                    }
                }

                if (productionPlan.OrdersToLineConformity.First().Orders.Count == 0)
                    throw new Exception("Нет заказов");

                int l = 0;

                // Теперь будем вставлять остальные заказы в план в зависимости от рецепта предыдущего заказа (пока все заказы не забьем в план)
                while (productionPlan.OrdersToLineConformity.First().Orders.Count < ordersToExecute.Count && l < 2000)
                {
                    // берем последний тип пленки в плане
                    FilmRecipe lastFilmRecipe = productionPlan.OrdersToLineConformity.First().Orders.Last().FilmRecipe;

                    // находим все перенастройки с этого рецепта (отсортируем их по возрастанию сразу)
                    List<ExtruderRecipeChange> recipeChanges = extruderRecipeChange.Where(change => change.From.Equals(lastFilmRecipe.Recipe)).OrderBy(change => change.Duration).ToList();

                    // идем от наименьшего времени перенастройки к большему, ищем заказы
                    for (int k = 0; k < recipeChanges.Count; k++)
                    {
                        // если есть заказы с таким типом пленки
                        if (ordersToExecute.Where(order => order.FilmRecipe.Recipe.Equals(recipeChanges[k].On)).Count() > 0)
                        {
                            // и такого типа пленки еще не было в плане
                            if (productionPlan.OrdersToLineConformity.First().Orders.Where(x => x.FilmRecipe.Recipe.Equals(recipeChanges[k].On)).Count() == 0)
                            {
                                // выбираем все заказы с таким типом пленки
                                List<Order> _orders = ordersToExecute.Where(or => or.FilmRecipe.Recipe.Equals(recipeChanges[k].On)).OrderBy(order => order.FilmRecipe.NozzleInsert).ThenBy(order => order.FilmRecipe.CalibrationDiameter).ThenBy(order => order.FilmRecipe.CoolingLip).ToList();

                                // кидаем их в план
                                for (int j = 0; j < _orders.Count; j++)
                                {
                                    productionPlan.OrdersToLineConformity.First().Orders.Add(_orders[j]);
                                }

                                break; // заказы нашлись - здесь больше делать нечего, смотрим следующий тип пленки (возвращаемся в while)
                            }
                        }
                    }

                    l++;
                }

                //productionPlan.OrdersToLineConformity.First().Orders.Remove(productionPlan.OrdersToLineConformity.First().Orders.Last());

                return productionPlan;
            }*/

            private OrdersOnExtruderLine BestProdactionPlan(Extruder extruderLine, List<Order> ordersToExecute, List<SliceLine> slinesBundle)
        {
            //берем все перенастройки
            List<ExtruderRecipeChange> extruderRecipeChange = db.ExtruderRecipeChanges.ToList();
            List<ExtruderCalibrationChange> extruderCalibrationChanges = db.ExtruderCalibrationChanges.ToList();
            List<ExtruderCoolingLipChange> extruderCoolingLipChanges = db.ExtruderCoolingLipChanges.ToList();
            List<ExtruderNozzleChange> extruderNozzleChanges = db.ExtruderNozzleChanges.ToList();
            
            OrdersOnExtruderLine OrdersToLineConformity = new OrdersOnExtruderLine() { Line = extruderLine };

            extruderRecipeChange = extruderRecipeChange.OrderByDescending(change => change.Duration).ToList();

            if (extruderRecipeChange.Count == 0)
                throw new Exception("Нет перенастроек");

            // для начала смотрим все перенастройки в поисках заказов, которые могли бы выполняться
            for (int i = 0; i < extruderRecipeChange.Count; i++)
            {
                // если в списке заказов есть хотя бы один заказ по такому рецепту
                if (ordersToExecute.Where(order => order.FilmRecipe.Recipe.Equals(extruderRecipeChange[i].On)).Count() > 0)
                {
                    // выбираем все заказы по такому рецепту
                    List<Order> orders = ordersToExecute.Where(or => or.FilmRecipe.Recipe.Equals(extruderRecipeChange[i].On)).OrderBy(order => order.FilmRecipe.NozzleInsert).ThenBy(order => order.FilmRecipe.CalibrationDiameter).ThenBy(order => order.FilmRecipe.CoolingLip).ToList();

                    // и вставляем их в план
                    for (int j = 0; j < orders.Count; j++)
                    {
                        OrdersToLineConformity.Orders.Add(orders[j]);
                    }

                    break; // заказы выбраны - сваливаем
                }
            }

            if (OrdersToLineConformity.Orders.Count == 0)
                throw new Exception("Нет заказов");

            int l = 0;

            // Теперь будем вставлять остальные заказы в план в зависимости от рецепта предыдущего заказа (пока все заказы не забьем в план)
            while (OrdersToLineConformity.Orders.Count < ordersToExecute.Count && l < 2000)
            {
                // берем последний тип пленки в плане
                FilmRecipe lastFilmRecipe = OrdersToLineConformity.Orders.Last().FilmRecipe;

                // находим все перенастройки с этого рецепта (отсортируем их по возрастанию сразу)
                List<ExtruderRecipeChange> recipeChanges = extruderRecipeChange.Where(change => change.From.Equals(lastFilmRecipe.Recipe)).OrderBy(change => change.Duration).ToList();

                // идем от наименьшего времени перенастройки к большему, ищем заказы
                for (int k = 0; k < recipeChanges.Count; k++)
                {
                    // если есть заказы с таким типом пленки
                    if (ordersToExecute.Where(order => order.FilmRecipe.Recipe.Equals(recipeChanges[k].On)).Count() > 0)
                    {
                        // и такого типа пленки еще не было в плане
                        if (OrdersToLineConformity.Orders.Where(x => x.FilmRecipe.Recipe.Equals(recipeChanges[k].On)).Count() == 0)
                        {
                            // выбираем все заказы с таким типом пленки
                            List<Order> _orders = ordersToExecute.Where(or => or.FilmRecipe.Recipe.Equals(recipeChanges[k].On)).OrderBy(order => order.FilmRecipe.NozzleInsert).ThenBy(order => order.FilmRecipe.CalibrationDiameter).ThenBy(order => order.FilmRecipe.CoolingLip).ToList();

                            // кидаем их в план
                            for (int j = 0; j < _orders.Count; j++)
                            {
                                OrdersToLineConformity.Orders.Add(_orders[j]);
                            }

                            break; // заказы нашлись - здесь больше делать нечего, смотрим следующий тип пленки (возвращаемся в while)
                        }
                    }
                }

                l++;
            }

            //productionPlan.OrdersToLineConformity.First().Orders.Remove(productionPlan.OrdersToLineConformity.First().Orders.Last());

            return OrdersToLineConformity;
        }
    }
}