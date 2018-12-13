using Algorithms.BruteForce;
using Algorithms.ObjectiveFunctions;
using System;
using System.Collections.Generic;
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
        private ExecutionTimeAndCost _timeAndCost;

        //показатель приоритетности
        public decimal Priority { get { return CalcPriority(); } }

        private decimal CalcPriority()
        {
            decimal result = 0;

            for (int i = 0; i < OrdersToLineConformity.Count; i++)
            {
                for (int j = 0; j < OrdersToLineConformity[i].Orders.Count; j++)
                {
                    result += OrdersToLineConformity[i].Orders[j].Priority * j;
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращает объект типа float, хранящего данные о затратах согласно требуемому критерию.
        /// Также сохраняет полученные результаты в float MaxWorkTime, float RetargetingTime и float WorkCosts
        /// </summary>
        public decimal GetWorkSpending(Costs costs, OptimizationCriterion criterion, AObjectiveFunction objectiveFunction)
        {
            decimal maxTime = 0,
                executionCost = 0,
                spending = 0;
            DateTime executionStart, executionEnd;

            RetargetingTime = 0;
            _timeAndCost = OrdersToLineConformity[0].CalculateExecutionTimeAndCost(costs, objectiveFunction);

            maxTime = _timeAndCost.ExecutionTime;
            RetargetingTime += _timeAndCost.RetargetingTime;
            executionStart = _timeAndCost.ExecutionStart;
            executionEnd = _timeAndCost.ExecutionEnd;
            executionCost = _timeAndCost.ExecutionCost;

            if (OrdersToLineConformity.Count > 1)
            {
                for (int i = 1; i < OrdersToLineConformity.Count; i++)
                {
                    _timeAndCost = OrdersToLineConformity[i].CalculateExecutionTimeAndCost(costs, objectiveFunction);
                    RetargetingTime += _timeAndCost.RetargetingTime;

                    // Дата начала у текущего заказа раньше той, что указана сейчас для всего пакета заказов.
                    if (executionStart.CompareTo(_timeAndCost.ExecutionStart) > 0)
                    {
                        executionStart = _timeAndCost.ExecutionStart;
                    }
                    // Дата завершения у текущего заказа позже той, что указана сейчас для всего пакета заказов.
                    if (executionEnd.CompareTo(_timeAndCost.ExecutionEnd) < 0)
                    {
                        executionEnd = _timeAndCost.ExecutionEnd;
                    }
                    executionCost += _timeAndCost.ExecutionCost;
                    maxTime = (decimal)executionEnd.Subtract(executionStart).TotalHours;
                }
            }

            if (MaxWorkTime < maxTime)
            {
                MaxWorkTime = maxTime;
            }
            WorkCosts = executionCost;

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

        // print changes between production plans
        public static string GetDifferences(ProductionPlan priviousPlan, ProductionPlan nextPlan)
        {
            int betterFlag = 0;
            StringBuilder result = new StringBuilder();

            decimal differenceMaxWorkTime = priviousPlan.MaxWorkTime - nextPlan.MaxWorkTime;

            if (differenceMaxWorkTime < 0)
            {
                result.AppendLine($"First plan is better in work time for {differenceMaxWorkTime} minuts.");
                betterFlag--;
            }
            else if (differenceMaxWorkTime > 0)
            {
                result.AppendLine($"Secound plan is better in work time for {differenceMaxWorkTime} minuts.");
                betterFlag++;
            }

            decimal differenceRetargetingTime = priviousPlan.RetargetingTime - nextPlan.RetargetingTime;

            if (differenceRetargetingTime < 0)
            {
                result.AppendLine($"First plan is better in retargeting time for {differenceRetargetingTime} minuts.");
                betterFlag--;
            }
            else if (differenceRetargetingTime > 0)
            {
                result.AppendLine($"Secound plan is better in retargeting time for {differenceRetargetingTime} minuts.");
                betterFlag++;
            }

            decimal differenceCost = priviousPlan.WorkCosts - nextPlan.WorkCosts;

            if (differenceCost < 0)
            {
                result.AppendLine($"First plan is better in cost for {differenceCost}.");
                betterFlag--;
            }
            else if (differenceCost > 0)
            {
                result.AppendLine($"Secound plan is better in cost for {differenceCost}.");
                betterFlag++;
            }

            if (priviousPlan.OrdersToLineConformity.Count - nextPlan.OrdersToLineConformity.Count != 0)
                result.AppendLine($"First plan use {priviousPlan.OrdersToLineConformity.Count} lines and secound plan use {nextPlan.OrdersToLineConformity.Count} lines.");


            for (int i = 0; i < priviousPlan.OrdersToLineConformity.Count; i++)
            {
                for (int j = 0; j < nextPlan.OrdersToLineConformity.Count; j++)
                    if (priviousPlan.OrdersToLineConformity[i].Line.ID == nextPlan.OrdersToLineConformity[j].Line.ID)
                    {
                        for (int k = 0; k < priviousPlan.OrdersToLineConformity[i].Orders; k++)
                        {
                            if (!nextPlan.OrdersToLineConformity[j].Orders.Contains(priviousPlan.OrdersToLineConformity[i].Orders[k]))
                                result.AppendLine($"Second plan has no order by ID { priviousPlan.OrdersToLineConformity[i].Ordders[k].ID.ToString() } on line { priviousPlan.OrdersToLineConformity[i].Line.Name }.");
                            else if (nextPlan.OrdersToLineConformity[j].Orders.IndexOf(priviousPlan.OrdersToLineConformity[i].Orders[k]) != k)
                                result.AppendLine($"In first plan order by ID { priviousPlan.OrdersToLineConformity[i].Ordders[k].ID.ToString() } was in {k} position but in second plan it has { nextPlan.OrdersToLineConformity[j].Orders.IndexOf(priviousPlan.OrdersToLineConformity[i].Orders[k]) } position.");
                        }
                    }
            }

            return result.ToString();
        }
    }
}