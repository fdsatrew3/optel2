using Optel2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.Genetic.Operators
{
    class CrossoverOperator
    {
        private int _firstChromosomeToCrossover = 0;
        private int _secondChromosomeToCrossover = 1;

        public void MadeCrossover(ref List<ProductionPlan> populations, int populationIndex)
        {
            Random rand = new Random();

            int indexOfChromosomeOutOfRange = -1,
                indexOfChromosomeNotOutOfRange = -1,
                temporalChromosomeNumber = 0,
                maxAttemptsToGetChromosome = 100,
                attamptsToGetChromosome = maxAttemptsToGetChromosome,
                numberOfLinesInPopulation = populations[populationIndex].OrdersToLineConformity.Count;

            // Номера заказов, который были перенесены в одностороннем порядке.
            List<int> indexesOfOneSideReplacedOrders = new List<int>();

            // Определяем пары хромосом, которые будут скрещиваться. Стараемся при этом добиться того, чтобы
            // каждый раз выбирается новая пара хромосом. Поскольку мы запрещаем кроссовер в случае всего одной
            // линии, ситуация с бесконечным подбором первой хромосомы невозможна: однако на всякий случай
            // ставим ограничение на количество попыток подбора новой хромосомы.
            // Если линий более 2, то также стараемся добиться нового номера для второй линии.

            do
            {
                temporalChromosomeNumber = rand.Next(0, numberOfLinesInPopulation);
                attamptsToGetChromosome--;
            } while (temporalChromosomeNumber == _firstChromosomeToCrossover && attamptsToGetChromosome > 0);
            _firstChromosomeToCrossover = temporalChromosomeNumber;

            do
            {
                _secondChromosomeToCrossover = rand.Next(0, numberOfLinesInPopulation);
            } while (_secondChromosomeToCrossover == _firstChromosomeToCrossover);

            // Создаём новую популяцию и копируем в неё ту, которая является родительской.
            _MaidNewPopulationFromParent(ref populations, populationIndex);

            // Перекладываем в новой популяции заказы между скрещиваемыми хромосомами.
            for (int i = 0; i < _CroossoverDotFinder(ref populations, populationIndex, _firstChromosomeToCrossover, _secondChromosomeToCrossover); i++)
            {
                // Выхода за пределы массива не наблюдается для обеих хромосом: просто меняем заказы
                // местами, если параметры линий это позволяют.
                if (!_OrderIndexOutOfRange(populations[populationIndex].OrdersToLineConformity[_firstChromosomeToCrossover].Orders, i) &&
                    !_OrderIndexOutOfRange(populations[populationIndex].OrdersToLineConformity[_secondChromosomeToCrossover].Orders, i))
                {
                    // Заказами можно поменяться.
                    if (populations[populationIndex].OrdersToLineConformity[_firstChromosomeToCrossover].Orders[i].CheckCompabilityWithLine(populations[populationIndex].OrdersToLineConformity[_secondChromosomeToCrossover].Line) &&
                        populations[populationIndex].OrdersToLineConformity[_secondChromosomeToCrossover].Orders[i].CheckCompabilityWithLine(populations[populationIndex].OrdersToLineConformity[_firstChromosomeToCrossover].Line))
                    {
                        populations[populations.Count - 1].OrdersToLineConformity[_firstChromosomeToCrossover].Orders[i] = populations[populationIndex].OrdersToLineConformity[_secondChromosomeToCrossover].Orders[i];
                        populations[populations.Count - 1].OrdersToLineConformity[_secondChromosomeToCrossover].Orders[i] = populations[populationIndex].OrdersToLineConformity[_firstChromosomeToCrossover].Orders[i];
                    }
                    // Поменяться заказами нельзя: они остаются на месте.
                    else
                    {
                        populations[populations.Count - 1].OrdersToLineConformity[_firstChromosomeToCrossover].Orders[i] = populations[populationIndex].OrdersToLineConformity[_firstChromosomeToCrossover].Orders[i];
                        populations[populations.Count - 1].OrdersToLineConformity[_secondChromosomeToCrossover].Orders[i] = populations[populationIndex].OrdersToLineConformity[_secondChromosomeToCrossover].Orders[i];
                    }
                }
                else
                {
                    // Индекс за пределами массива для первой хромосомы.
                    if (_OrderIndexOutOfRange(populations[populationIndex].OrdersToLineConformity[_firstChromosomeToCrossover].Orders, i) &&
                        !_OrderIndexOutOfRange(populations[populationIndex].OrdersToLineConformity[_secondChromosomeToCrossover].Orders, i))
                    {
                        indexOfChromosomeOutOfRange = _firstChromosomeToCrossover;
                        indexOfChromosomeNotOutOfRange = _secondChromosomeToCrossover;
                    }
                    else if (_OrderIndexOutOfRange(populations[populationIndex].OrdersToLineConformity[_secondChromosomeToCrossover].Orders, i) &&
                        !_OrderIndexOutOfRange(populations[populationIndex].OrdersToLineConformity[_firstChromosomeToCrossover].Orders, i))
                    // Индекс за пределами массива для второй хромосомы.
                    {
                        indexOfChromosomeOutOfRange = _secondChromosomeToCrossover;
                        indexOfChromosomeNotOutOfRange = _firstChromosomeToCrossover;
                    }
                    // Оба за пределами массива.
                    else
                    {
                        break;
                    }

                    // Передать заказ между линиями можно: делаем это.
                    if (populations[populationIndex].OrdersToLineConformity[indexOfChromosomeNotOutOfRange].Orders[i].CheckCompabilityWithLine(populations[populationIndex].OrdersToLineConformity[indexOfChromosomeOutOfRange].Line))
                    {
                        int newPrt = populations[populations.Count - 1].OrdersToLineConformity[indexOfChromosomeOutOfRange].Orders.Count;
                        // Создаём новый элемент, дабы не вылететь за пределы массива.
                        populations[populations.Count - 1].OrdersToLineConformity[indexOfChromosomeOutOfRange].Orders.Add(new Order());
                        // Помещаем заказ в только что созданную ячейку массива.
                        populations[populations.Count - 1].OrdersToLineConformity[indexOfChromosomeOutOfRange].Orders[newPrt] = populations[populationIndex].OrdersToLineConformity[indexOfChromosomeNotOutOfRange].Orders[i];
                        // Запоминаем индекс перемещённого заказа: далее он будет удалён.
                        indexesOfOneSideReplacedOrders.Add(i);
                    }
                    // Передать заказы между линиями нельзя.
                    else
                    {
                        populations[populations.Count - 1].OrdersToLineConformity[indexOfChromosomeNotOutOfRange].Orders[i] = populations[populationIndex].OrdersToLineConformity[indexOfChromosomeNotOutOfRange].Orders[i];
                    }
                }
            }

            // Удаляем из хромосомы, которая имела бОльший размер при скрещивании, односторонне
            // перенесённые заказы.
            if (indexesOfOneSideReplacedOrders.Count > 0)
            {
                List<Order> remainingOrders = new List<Order>();
                for (int i = 0; i < populations[populations.Count - 1].OrdersToLineConformity[indexOfChromosomeNotOutOfRange].Orders.Count; i++)
                {
                    if (!indexesOfOneSideReplacedOrders.Contains(i))
                    {
                        remainingOrders.Add(populations[populations.Count - 1].OrdersToLineConformity[indexOfChromosomeNotOutOfRange].Orders[i]);
                    }
                }
                populations[populations.Count - 1].OrdersToLineConformity[indexOfChromosomeNotOutOfRange].Orders = remainingOrders;
            }
        }

        private void _MaidNewPopulationFromParent(ref List<ProductionPlan> populations, int populationIndex)
        {
            populations.Add(new ProductionPlan());
            populations[populations.Count - 1].OrdersToLineConformity = new List<OrdersOnExtruderLine>();
            for (int i = 0; i < populations[populationIndex].OrdersToLineConformity.Count; i++)
            {
                populations[populations.Count - 1].OrdersToLineConformity.Add(new OrdersOnExtruderLine());
                populations[populations.Count - 1].OrdersToLineConformity[i].Line = populations[populationIndex].OrdersToLineConformity[i].Line;
                populations[populations.Count - 1].OrdersToLineConformity[i].Orders = new List<Order>(populations[populationIndex].OrdersToLineConformity[i].Orders);
            }
        }

        private int _CroossoverDotFinder(ref List<ProductionPlan> populations, int populationIndex, int first, int second)
        {
            Random rand = new Random();
            int dot = 0, chromosomeToGetDot = 0;

            // Возможна ситуация, при которой одна из линий вообще не имеет заказов:
            // в таком случае отказываемся от неё.
            if (populations[populationIndex].OrdersToLineConformity[first].Orders.Count == 0)
            {
                chromosomeToGetDot = second;
            }
            else if (populations[populationIndex].OrdersToLineConformity[second].Orders.Count == 0)
            {
                chromosomeToGetDot = first;
            }
            else
            {
                chromosomeToGetDot = (rand.Next(0, 2) == 1) ? first : second;
            }

            dot = rand.Next(0, populations[populationIndex].OrdersToLineConformity[chromosomeToGetDot].Orders.Count);
            //   System.Console.WriteLine(first + " " + second + " " + dot);
            return dot;
        }

        private bool _OrderIndexOutOfRange(List<Order> orderToCheck, int index)
        {
            try
            {
                var tmp = orderToCheck[index];
                return false;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return true;
            }
        }
    }
}
