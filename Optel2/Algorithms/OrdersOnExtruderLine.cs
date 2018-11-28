using Algorithms.ObjectiveFunctions;
using Optel2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class OrdersOnExtruderLine
    {
        public Extruder Line;
        public List<Order> Orders = new List<Order>();
        public decimal RetargetingTimeOnLine { private set; get; }
        public float ExecutionTimeOnLine => _GetExecutionTimeOnLine();

        public ExecutionTimeAndCost CalculateExecutionTimeAndCost(Costs costs, AObjectiveFunction objectiveFunction)
        {
            ExecutionTimeAndCost executionData = objectiveFunction.GetExecutionTimeAndCost(costs, Line, Orders);
            RetargetingTimeOnLine = executionData.RetargetingTime;
            return executionData;
        }

        private float _GetExecutionTimeOnLine()
        {
            float executionTime = 0;

            if (Orders.Count > 1)
            {
                executionTime = (float)(Orders[Orders.Count - 1].PlanedEndDate.Subtract(Orders[0].PlanedStartDate)).TotalHours;
            }
            else if (Orders.Count == 1)
            {
                executionTime = (float)(Orders[0].PlanedEndDate.Subtract(Orders[0].PlanedStartDate)).TotalHours;
            }

            return executionTime;
        }
    }
}
