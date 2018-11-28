using Optel2.Models;
using System;
using System.Collections.Generic;
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
            DateTime executionStart = DateTime.Now, executionEnd = DateTime.Now;
            decimal totalWorkTime = 0, totalRetargetingTime = 0, workCost = 0;
            // Есть заказы на линии.
            if (Orders.Count > 0)
            {
                // Прочие заказы.
                if (Orders.Count > 1)
                {
                    for (int i = 1; i < Orders.Count; i++)
                    {

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
    }
}
