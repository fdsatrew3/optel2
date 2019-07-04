using Algorithms.BruteForce;
using Algorithms.ObjectiveFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class ProductionPlan
    {
        // Порядковый номер линии.
        public int LineNumber;

        // Порядковый номер резательной машины.
        public int CuttingMachineNumber;

        // Дата начала выполнения заказа (включается в себя дату и время запуска заказа на производство).
        public float StartDateOfExecution;

        // Интервал времени, требуемый на выполнения заказа на j-ой линии.
        public float TimeGapToExecuteOrder = 0;

        // Количество заказов, выполняемых на j-ой линии в текущем расписании.
        public int CurrentNumberOfOrdersOnLine;

        // Порядковый номер выполнения заказа на j-ой линии в текущем расписании, от 1 до L.
        public int CurrentExecutedOrder;

        /// Список с соответствием заказ-линия.
        public List<OrdersOnExtruderLine> OrdersToLineConformity;
        public decimal MaxWorkTime { private set; get; }
        public decimal RetargetingTime { private set; get; }
        public decimal WorkCosts { private set; get; }
        public enum OptimizationCriterion { Time, Cost }
        //private ExecutionTimeAndCost _timeAndCost;

        public ProductionPlan()
        {
            // ?
        }

        public ProductionPlan(ProductionPlan plan)
        {
            OrdersToLineConformity = new List<OrdersOnExtruderLine>();

            for (int i = 0; i < plan.OrdersToLineConformity.Count; i++)
            {
                OrdersToLineConformity.Add(new OrdersOnExtruderLine());

                OrdersToLineConformity.Last().Line = plan.OrdersToLineConformity[i].Line;
                OrdersToLineConformity.Last().Orders = new List<Optel2.Models.Order>();

                for (int j = 0; j < plan.OrdersToLineConformity[i].Orders.Count; j++)
                {
                    OrdersToLineConformity.Last().Orders.Add(plan.OrdersToLineConformity[i].Orders[j]);
                }
            }
        }

        /// <summary>
        /// Возвращает объект типа float, хранящего данные о затратах согласно требуемому критерию.
        /// Также сохраняет полученные результаты в float MaxWorkTime, float RetargetingTime и float WorkCosts
        /// </summary>
        public decimal GetWorkSpending(Costs costs, OptimizationCriterion criterion, AObjectiveFunction objectiveFunction)
        {
            decimal spending = 0, maxTime = 0, executionCost = 0;
            decimal curTime = 0;
            for (int i = 0; i < OrdersToLineConformity.Count; i++)
            {
                curTime = OrdersToLineConformity[i].CalculateExecutionTimeAndCost(costs, objectiveFunction).ExecutionTime;

                /*
                for (int j = 0; j < OrdersToLineConformity[i].Orders.Count; j++)
                {
                    curTime += OrdersToLineConformity[i].Orders[j].PredefinedTime + OrdersToLineConformity[i].Orders[j].PredefinedRetargetTime;
                }*/
                if (curTime > maxTime)
                {
                    maxTime = curTime;
                }
                curTime = 0;
                executionCost += OrdersToLineConformity[i].CalculateExecutionTimeAndCost(costs, objectiveFunction).ExecutionCost;
            }
            switch (criterion)
            {
                case OptimizationCriterion.Time:
                    spending = maxTime;
                    break;
                case OptimizationCriterion.Cost:
                    spending = executionCost;
                    break;
                default:
                    throw new System.Exception("Unknown goal: " + criterion);
            }
            return spending;
        }

        static public ProductionPlan GetProductionPlan(List<Optel2.Models.Order> orders, List<Optel2.Models.Extruder> extruder, DateTime plannedStartDate)
        {
            int[] ordersNum = new int[] { 101586,
101585,
101596,
101597,
101587,
101589,
101649,
101559,
101543,
101650,
400134,
101590,
101651,
101648,
101647,
101646,
101652,
101663,
101653,
101613,
101614,
101620,
101632,
101621,
101615,
101633,
101662,
101616,
101627,
101634,
101654,
101617,
101635,
101628,
101626,
101618,
101636,
101637,
101619,
101629,
101638,
101630,
101639,
101631,
101640,
101656,
101657,
101664,
101655,
101641,
101658,
101644,
101659,
101642,
101660,
101643,
101661,
101645 }; 
            ProductionPlan productionPlan = new ProductionPlan();
            productionPlan.OrdersToLineConformity = new List<OrdersOnExtruderLine>();
            productionPlan.OrdersToLineConformity.Add(new OrdersOnExtruderLine() { Line = extruder[0] });

            for (int i = 0; i < ordersNum.Length; i++)
            {
                productionPlan.OrdersToLineConformity[0].Orders.Add(orders.Where(order => order.OrderNumber.Equals(ordersNum[i].ToString())).First());
                if (i == 0)
                {
                    productionPlan.OrdersToLineConformity[0].Orders[i].PlanedStartDate = plannedStartDate;
                    productionPlan.OrdersToLineConformity[0].Orders[i].PlanedEndDate = plannedStartDate.AddSeconds(productionPlan.OrdersToLineConformity[0].Orders[i].PredefinedTime);
                }
                else
                {
                    productionPlan.OrdersToLineConformity[0].Orders[i].PlanedStartDate = productionPlan.OrdersToLineConformity[0].Orders[i - 1].PlanedEndDate;
                    productionPlan.OrdersToLineConformity[0].Orders[i].PlanedEndDate = productionPlan.OrdersToLineConformity[0].Orders[i].PlanedStartDate.AddSeconds(productionPlan.OrdersToLineConformity[0].Orders[i].PredefinedTime).AddSeconds(productionPlan.OrdersToLineConformity[0].Orders[i].PredefinedRetargetTime);
                }
            }

            return productionPlan;
        }
    }
}