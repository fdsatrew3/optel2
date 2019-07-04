using Algorithms.ObjectiveFunctions;
using Optel2.Algorithms;
using Optel2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algorithms.ProductionPlan;

namespace Algorithms.BruteForce
{
    public class BruteForceAlgorithm
    {
        // Поступивший пакет заказов.
        private List<Order> _ordersPackage;
        // Набор экструзионных/каландровых линий.
        private List<Extruder> _eklinesBundle;
        // Набор резательных машин.
        private List<SliceLine> _slinesBundle;

        // Наборы заказов, которые невозможно выполнить (не помещаются на линию).
        private List<Order> _ordersWithoutLines;

        // Информация о затратах на производство.
        private Costs _productionCosts;

        // Цель оптимизации (по времени или по стоимости).
        private OptimizationCriterion _optimizationCriterion;
        private AObjectiveFunction _objectiveFunction;

        // В ЭТУ ШТУКУ БУДУТ ЗАПИСЫВАТЬСЯ САМЫЕ ОПТИМАЛЬНЫЕ ПЛАНЫ, ТАК ЧТО В СВОЙСТВО ИЗМЕНЕНИЯ ЭТОГО ПОЛЯ МОЖНО ДОБАВИТЬ ФУНКЦИЮ, ВЫВОДЯЩУЮ ПРОМЕЖУТОЧНЫЙ ПЛАН НА ЭКРАН
        public ProductionPlan SelectedPlan { get; set; }

        // Дерево решение
        //public List<ProductionPlan> Tree { get; set; }
        public List<Decision> DecisionTree { get; set; }
        private bool _needTree;

        public async Task<ProductionPlan> Start(List<Extruder> extruderLines, List<Order> ordersToExecute, List<SliceLine> slinesBundle, Costs productionCosts, OptimizationCriterion criterion, AObjectiveFunction function, bool _needTree = false)
        {
            this._needTree = _needTree;

            if (this._needTree)
            {
                // Tree = new List<ProductionPlan>();
                DecisionTree = new List<Decision>();
            }

            SelectedPlan = new ProductionPlan();

            _optimizationCriterion = criterion;
            _objectiveFunction = function;

            Random rand = new Random();

            // Заполняем переменные под заказы, линии и стоимость работ полученными на вход данными.
            _ordersPackage = ordersToExecute;
            _eklinesBundle = extruderLines;
            _slinesBundle = slinesBundle;
            _productionCosts = productionCosts;

            // Проверяем данные о заказах и линиях на совместимость и возможность выполнения.
            _CheckOrdersOnOpportunityOfExecute();

            int[] _variations = new int[ordersToExecute.Count()];

            CreateListForce(0, ref _variations);

            if (_needTree)
            {
                DecisionTree = DecisionTree.OrderByDescending(tree => tree.FunctionValue).ToList();
                //optimalPlan = DecisionTree.OrderBy(tree => tree.FunctionValue).First().Plan;
            }

            BestAlgoritm bestAlgoritm = new BestAlgoritm();
            ProductionPlan productionPlan = bestAlgoritm.Start(extruderLines, ordersToExecute, slinesBundle);

            if (DecisionTree.Last().Plan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) > productionPlan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction))
            {
                if (_needTree)
                    DecisionTree.Add(new Decision { Plan = productionPlan, FunctionValue = productionPlan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) });

                SelectedPlan = productionPlan;
            }

            return SelectedPlan;
        }

        private void _CheckOrdersOnOpportunityOfExecute()
        {
            bool orderHasLinesToExecute = false;
            _ordersWithoutLines = new List<Order>();

            for (int i = 0; i < _ordersPackage.Count; i++)
            {
                // Проверка: есть хоть одна линия, на которой можно выполнить данный заказ.
                foreach (Extruder ekl in _eklinesBundle)
                {
                    if (_ordersPackage[i].CheckCompabilityWithLine(ekl))
                    {
                        orderHasLinesToExecute = true;
                        break;
                    }
                }
                // Заказ не выполнить ни на одной из линий.
                if (!orderHasLinesToExecute)
                {
                    OrdersWithoutLines.Add(_ordersPackage[i]);
                }
            }
        }

        private void CreateListForce(int iteration, ref int[] len)
        {
            for (len[iteration] = 0; len[iteration] < len.Length; len[iteration]++)
            {
                if (IsInLen(iteration, len)) // избавляемся от повторений
                    continue;
                else if (iteration < len.Length - 1)
                    CreateListForce(++iteration, ref len);
                else
                {
                    ProductionPlan newPlan = CreatePlanByArray(len); // по массиву перебора создаем план
                    ProductionPlan oldPlan = SelectedPlan; // сохраняем старый план
                    SelectedPlan = ChooseBestPlan(new List<ProductionPlan>() { newPlan, SelectedPlan });

                    if (_needTree)
                    {
                        if (DecisionTree.Count == 0)
                            DecisionTree.Add(new Decision { Plan = SelectedPlan, FunctionValue = SelectedPlan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) });
                        if (DecisionTree.Last().Plan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) > SelectedPlan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction))
                            DecisionTree.Add(new Decision { Plan = SelectedPlan, FunctionValue = SelectedPlan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) });
                    }
                }
            }
        }

        private bool IsInLen(int iteration, int[] len)
        {
            for (int i = 0; i < iteration; i++)
                if (len[i] == len[iteration])
                    return true;

            return false;
        }

        private ProductionPlan CreatePlanByArray(int[] len)
        {
            ProductionPlan result = new ProductionPlan();
            result.OrdersToLineConformity = new List<OrdersOnExtruderLine>();

            // Создаём набор линий для данной популяции.
            for (int j = 0; j < _eklinesBundle.Count; j++)
            {
                result.OrdersToLineConformity.Add(new OrdersOnExtruderLine());
                result.OrdersToLineConformity[j].Line = _eklinesBundle[j];
            }

            int i = 0;

            while (i < len.Length)
            {
                result.OrdersToLineConformity[i % _eklinesBundle.Count].Orders.Add(_ordersPackage[len[i]]);
                i++;
            }

            /*
            for (int j = 0; j < len.Length; j++)
            {
                ChooseOrderToLine(result, _ordersPackage[len[j]]);
            }
            */
            return result;
        }

        private void ChooseOrderToLine(ProductionPlan plan, Order order)
        {
            int choosenEKLineNumber;
            bool orderPlacedOnLine = false;

            // берем все линии и находим самые менее загруженные
            var linesWithOrders = plan.OrdersToLineConformity.OrderBy(line => line.ExecutionTimeOnLine).ToList();

            foreach (var line in linesWithOrders)
            {
                // Заказ можно выполнить на линии.
                if (order.CheckCompabilityWithLine(line.Line))
                {
                    choosenEKLineNumber = plan.OrdersToLineConformity.IndexOf(line);
                    plan.OrdersToLineConformity[choosenEKLineNumber].Orders.Add(order);
                    orderPlacedOnLine = true;
                    break;
                }
            }

            if (!orderPlacedOnLine)
                OrdersWithoutLines.Add(order);
        }

        private ProductionPlan ChooseBestPlan(List<ProductionPlan> plan)
        {
            ProductionPlan newPlan;

            try
            {
                newPlan = plan.OrderBy(spending => spending.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction)).First();
            }
            catch (Exception)
            { newPlan = plan[0]; }

            return newPlan;
        }

        public List<Order> OrdersWithoutLines
        {
            get
            {
                return _ordersWithoutLines;
            }
        }
    }
}