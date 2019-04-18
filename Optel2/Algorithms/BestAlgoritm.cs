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
        private OptelContext db = new OptelContext();

        public void Dispose()
        {
            db.Dispose();
        }

        public ProductionPlan Start(List<Extruder> extruderLine, List<Order> ordersToExecute, List<SliceLine> slinesBundle)
        {
            // работаем только для одной линии (например, первой)
            Extruder extruderLines = extruderLine.First();

            //берем все перенастройки
            List<ExtruderRecipeChange> extruderRecipeChange = db.ExtruderRecipeChanges.ToList();

            //создаем план
            ProductionPlan productionPlan = new ProductionPlan();
            productionPlan.OrdersToLineConformity = new List<OrdersOnExtruderLine>();
            productionPlan.OrdersToLineConformity.Add(new OrdersOnExtruderLine() { Line = extruderLines });
            productionPlan.OrdersToLineConformity.First().Orders = new List<Order>();

            // выбираем все перенастройки, на выбранной линии 
            // сразу сортируем от максимального времени перенастраивания (так как для первого заказа перенастройка не учитывается, то можно запихнуть наибольшее время туда)
            //extruderRecipeChange = extruderRecipeChange.Where(ch => !(ch.Extruder is null)).ToList();
            //extruderRecipeChange = extruderRecipeChange.Where(ch => ch.Extruder.Equals(extruderLines)).ToList();
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
                    List<Order> orders = ordersToExecute.Where(or => or.FilmRecipe.Recipe.Equals(extruderRecipeChange[i].On)).OrderBy(order => order.Width).ToList();

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
                            List<Order> _orders = ordersToExecute.Where(or => or.FilmRecipe.Recipe.Equals(recipeChanges[k].On)).OrderBy(order => order.Width).ToList();

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
        }
    }
}