using Algorithms;
using Algorithms.Genetic.Operators;
using Algorithms.ObjectiveFunctions;
using Optel2.Algorithms;
using Optel2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algorithms.ProductionPlan;

namespace GenetycAlgorithm
{
    public class GeneticAlgorithm
    {
        private const int _negativePropability = -1;
        private const int _maximumPropability = 101;
        private const decimal c_oneHundredPercents = 1.0m;
        private const decimal c_zeroPercents = 0.0m;

        // Максимальное число популяций.
        private int _maxPopulationsAmount;

        // Максимальный размер выборки.      
        private int _maxAmountOfSelection;

        // Вероятность кроссовера.
        private decimal _crossoverPropability = 95;

        // Вероятность мутации.
        private decimal _mutationPropability = 15;
        private decimal _percentOfMutableGens = 0.5m;

        // Количество итераций.
        private int _numberOfGAIterations = 7;

        // Поступивший пакет заказов.
        private List<Order> _ordersPackage;
        // Набор экструзионных/каландровых линий.
        private List<Extruder> _eklinesBundle;
        // Набор резательных машин.
        private List<SliceLine> _slinesBundle;

        // Наборы заказов, которые невозможно выполнить (не помещаются на линию).
        private List<Order> _ordersWithoutLines;

        private List<ProductionPlan> _populations;

        // Информация о затратах на производство.
        private Costs _productionCosts;

        // Защита от стагнации (более подробно алгоритм расписан в _StagnationDefense(1)).
        private List<decimal> _lastProductionSpendingInfo;
        private const int c_lastProductionTimesLimit = 5;
        // Минимально приемлемая скорость схождения (в процентах).
        private const decimal _minimalAcceptableConvergenceSpeed = 5;

        // Цель оптимизации (по времени или по стоимости).
        private OptimizationCriterion _optimizationCriterion;
        private AObjectiveFunction _objectiveFunction;

        // Дерево решений
        // public List<ProductionPlan> Tree { get; set; }
        public List<Decision> DecisionTree { get; set; }
        private bool _isNeedTree;

        public async Task<ProductionPlan> Start(List<Extruder> extruderLines, List<Order> ordersToExecute, List<SliceLine> slinesBundle, Costs productionCosts, OptimizationCriterion criterion, AObjectiveFunction function,
                                    int maxPopulation, int numberOfGAiterations, int maxSelection, bool _needTree = false, int mutationPropability = 15, decimal percentOfMutableGens = 0.5m, int crossoverPropability = 95)
        {
            _isNeedTree = _needTree;

            if (_isNeedTree)
            {
                // Tree = new List<ProductionPlan>();
                DecisionTree = new List<Decision>();
            }

            ProductionPlan optimalPlan = new ProductionPlan();
            CrossoverOperator crossoverOperator = new CrossoverOperator();
            MutationOperator mutationOperator = new MutationOperator();

            _numberOfGAIterations = numberOfGAiterations;
            _optimizationCriterion = criterion;
            _objectiveFunction = function;

            Random rand = new Random();

            List<int> selectedPopulations = new List<int>();
            _lastProductionSpendingInfo = new List<decimal>();

            _maxPopulationsAmount = maxPopulation;
            _maxAmountOfSelection = (maxSelection <= maxPopulation) ? maxSelection : maxPopulation;
            _percentOfMutableGens = (percentOfMutableGens < c_zeroPercents ||
                                    _percentOfMutableGens > c_oneHundredPercents) ? rand.Next(0, 100) / 100 : percentOfMutableGens;

            // Заполняем переменные под заказы, линии и стоимость работ полученными на вход данными.
            _ordersPackage = ordersToExecute;
            _eklinesBundle = extruderLines;
            _slinesBundle = slinesBundle;
            _productionCosts = productionCosts;

            // Всего одна линия: кроссовер бессмысленен. Остаётся только мутация: ставим вероятность
            // кроссовера отрицательной, а вероятность мутации - как можно выше.
            if (_eklinesBundle.Count == 1)
            {
                _crossoverPropability = _negativePropability;
                _mutationPropability = _maximumPropability;
            }
            else
            {
                _crossoverPropability = (crossoverPropability > _negativePropability &&
                                        crossoverPropability < _maximumPropability) ? crossoverPropability : _crossoverPropability;
                _mutationPropability = (mutationPropability > _negativePropability &&
                    mutationPropability < _maximumPropability) ? mutationPropability : _mutationPropability;
            }

            // Проверяем данные о заказах и линиях на совместимость и возможность выполнения.
            _CheckOrdersOnOpportunityOfExecute();

            _MadeStarterPopulations();

            // Начинаем "разводить" особей.           
            for (int i = 0; i < _numberOfGAIterations; i++)
            {
                Debug.Print(i + "/" + _numberOfGAIterations);
                // Обрабатываем популяции хромосом (каждая популяция имеет по EKLinesAmount хромосом).
                // Не все популяции обрабатываются: выбираются случайные _maxAmountOfSelection
                // популяций, которые могут быть обработаны.
                selectedPopulations = _SelectPopulationsToProceed();

                for (int populationIndex = 0; populationIndex < _populations.Count; populationIndex++)
                {
                    if (!selectedPopulations.Contains(populationIndex))
                    {
                        continue;
                    }

                    if (rand.Next(0, _maximumPropability) < _crossoverPropability)
                    {
                        crossoverOperator.MadeCrossover(ref _populations, populationIndex);
                        /*
                        if (_isNeedTree)
                            DecisionTree.Add(new Decision {Parent = _populations[populationIndex], Iteration = i + 1, Operation = Decision.OperationType.Crossover, Plan = _populations.Last(), FunctionValue = _populations.Last().GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) });
                    */
                    }

                    if (rand.Next(0, _maximumPropability) < _mutationPropability)
                    {
                        mutationOperator.MadeMutation(ref _populations, populationIndex, _percentOfMutableGens);
                        /*
                        if (_isNeedTree)
                            DecisionTree.Add(new Decision {Parent = _populations[populationIndex], Iteration = i + 1, Operation  = Decision.OperationType.Mutation, Plan = _populations.Last(), FunctionValue = _populations.Last().GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) });
                    */
                    }
                }

                if (optimalPlan.OrdersToLineConformity == null)
                {
                    optimalPlan = _GetBestGens();
                }
                else
                {
                    // Выбор лучшего плана из всех популяций для данной итерации.
                    if (_GetBestGens().GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) < optimalPlan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction))
                    {
                        optimalPlan = _GetBestGens();
                    }
                }
                if (_isNeedTree)
                {
                    DecisionTree.Add(new Decision { Plan = optimalPlan, FunctionValue = optimalPlan.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) });
                }
                // Сохраняем только _maxPopulationsAmount популяций с текущей итерации.
                _SaveOnlyBestPopulations();

                _StagnationDefense(optimalPlan);

                /*     if (_needTree)
                         Tree.Add(new ProductionPlan(optimalPlan)); // добавляем в дерево решений текущий план
                         */
            }

            return optimalPlan;
        }

        private List<int> _SelectPopulationsToProceed()
        {
            List<int> selectedPopulations = new List<int>();
            int populationNumber = 0;
            Random rand = new Random();

            for (int i = 0; i < _maxAmountOfSelection; i++)
            {
                populationNumber = rand.Next(0, _populations.Count);
                while (selectedPopulations.Contains(populationNumber))
                {
                    populationNumber = rand.Next(0, _populations.Count);
                }
                selectedPopulations.Add(populationNumber);
            }

            return selectedPopulations;
        }

        private void _StagnationDefense(ProductionPlan currentPopulation)
        {
            decimal spendingAddition = 0;

            // Считаем, что самый первый план в списке имел худшее время. Поэтому его
            // потомки должны иметь лучший результат (то есть [предыдущие затраты] - [новые затраты] > 0).
            // Если же результат вычитания меньше или равен нулю, то это значит, что
            // популяция находится в стагнации и необходима встряска. Также встряска
            // может потребоваться, если время стало ненамного лучше.

            if (_numberOfGAIterations > c_lastProductionTimesLimit &&
                _numberOfGAIterations > 1)
            {
                _lastProductionSpendingInfo.Add(currentPopulation.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction));

                if (_lastProductionSpendingInfo.Count >= c_lastProductionTimesLimit)
                {
                    spendingAddition = _lastProductionSpendingInfo[0] - _lastProductionSpendingInfo[_lastProductionSpendingInfo.Count - 1];

                    if (spendingAddition <= 0 ||
                        (c_oneHundredPercents - Math.Abs(spendingAddition) / _lastProductionSpendingInfo[0]) < _minimalAcceptableConvergenceSpeed)
                    {
                        _lastProductionSpendingInfo.Clear();
                        _MadeStarterPopulations();
                    }
                }
            }
        }

        private void _MadeStarterPopulations()
        {
            _populations = new List<ProductionPlan>();
            _PopulationsInitialisation();
        }

        // Общее число хромосом в популяции не должно превышать число линий.
        private void _PopulationsInitialisation()
        {
            // Создаём популяции хромосом. Каждая популяция - это набор линий, которые
            // содержат множество заказов. Один и тот же закза не может быть сразу на
            // нескольких линиях в одной популяции!
            for (int i = 0; i < _maxPopulationsAmount; i++)
            {
                _populations.Add(new ProductionPlan());
                _populations[_populations.Count - 1].OrdersToLineConformity = new List<OrdersOnExtruderLine>();

                // Создаём набор линий для данной популяции.
                for (int j = 0; j < _eklinesBundle.Count; j++)
                {
                    _populations[i].OrdersToLineConformity.Add(new OrdersOnExtruderLine());
                    _populations[i].OrdersToLineConformity[j].Line = _eklinesBundle[j];
                }

                FisherYates.Shuffle(_ordersPackage);
                // Раскидываем имеющиеся заказы по линиям.
                foreach (Order order in _ordersPackage)
                {
                    // Данный заказ не выполнить ни на одной из линий: пропускаем его.
                    if (_ordersWithoutLines.Contains(order))
                    {
                        continue;
                    }

                    // Случайным образом выбираем линию для выполнения заказа.
                    // Возможна ситуация, при которой заказ не подходит для выбранной линии:
                    // в таком случае выбираем иную. Если заказ не удалось разместить за
                    // maxCountsToSetOrder попыток, то выбираем первую линию, которая может
                    // принять заказ.
                    ChooseOrderToLine(_populations[i], order); // _ChooseLineToOrder(_populations[i], order);
                }
            }
            //if (_isNeedTree)
            //    DecisionTree.Add(new Decision { Parent = null, Iteration = 0, Operation = Decision.OperationType.CreatePopulation, Plan = _populations.Last(), FunctionValue = _populations.Last().GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) });
        }

        // ТЕСТОВАЯ ФУНКЦИЯ
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

        /*
        /// <summary>
        /// Определяет соответствие между заказами и линиями в популяции.
        /// </summary>
        private void _ChooseLineToOrder(ProductionPlan population, Order order)
        {
            bool orderPlacedOnLine = false;
            Random rand = new Random();
            int choosenEKLineNumber = 0;
            // OrdersToLineConformity на момент инициализации maxAttemptsToSetOrder содержит
            // только список линий данной популяции.
            int maxAttemptsToSetOrder = population.OrdersToLineConformity.Count;

            while (!orderPlacedOnLine)
            {
                // выбираем менее загруженную линию
                choosenEKLineNumber = population.OrdersToLineConformity.IndexOf(population.OrdersToLineConformity.OrderBy(line => line.ExecutionTimeOnLine).First());//rand.Next(0, population.OrdersToLineConformity.Count);
                // Заказ можно выполнить на линии.
                if (order.CheckCompabilityWithEKLine(population.OrdersToLineConformity[choosenEKLineNumber].Line))
                {
                    population.OrdersToLineConformity[choosenEKLineNumber].Orders.Add(order);
                    orderPlacedOnLine = true;
                }
                else
                {
                    maxAttemptsToSetOrder--;
                    // Ищем первую подходящую линию.
                    if (maxAttemptsToSetOrder < 0)
                    {
                        for (int j = 0; j < population.OrdersToLineConformity.Count; j++)
                        {
                            if (order.CheckCompabilityWithEKLine(population.OrdersToLineConformity[j].Line))
                            {
                                population.OrdersToLineConformity[j].Orders.Add(order);
                                break;
                            }
                        }

                        orderPlacedOnLine = true;
                    }
                    continue;
                }
            }
        }
        */
        private ProductionPlan _GetBestGens()
        {
            _populations = _populations.OrderBy(spending => spending.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction)).ToList();
            return _populations[0];
        }

        private void _SaveOnlyBestPopulations()
        {
            if (_populations.Count > _maxPopulationsAmount)
            {
                _populations = _populations.OrderBy(spending => spending.GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction)).ToList();
                _populations = _populations.GetRange(0, _maxPopulationsAmount);

      //          if (_isNeedTree)
      //          {
      //              DecisionTree.Add(new Decision { Plan = _populations.First(), FunctionValue = _populations.First().GetWorkSpending(_productionCosts, _optimizationCriterion, _objectiveFunction) });
      //          }
            }
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
                    _ordersWithoutLines.Add(_ordersPackage[i]);
                }
            }
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