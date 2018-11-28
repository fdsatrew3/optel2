using Optel2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.Genetic.Operators
{
    class MutationOperator
    {
        public void MadeMutation(ref List<ProductionPlan> populations, int populationIndex, decimal percentOfMutableGens)
        {
            Order temporalOrder = null;
            Random rand = new Random();
            int firstOrderIndex = 0,
                secondOrderIndex = 0,
                numberOfGenParesForMutation = 0;

            for (int i = 0; i < populations[populationIndex].OrdersToLineConformity.Count; i++)
            {
                // На линии 1 заказ или его вообще нет - пропадает смысл в мутации.
                if (populations[populationIndex].OrdersToLineConformity[i].Orders.Count < 2)
                {
                    continue;
                }

                numberOfGenParesForMutation = (int)Math.Floor(populations[populationIndex].OrdersToLineConformity[i].Orders.Count * percentOfMutableGens);

                while (numberOfGenParesForMutation > 0)
                {
                    firstOrderIndex = rand.Next(populations[populationIndex].OrdersToLineConformity[i].Orders.Count);
                    do
                    {
                        secondOrderIndex = rand.Next(populations[populationIndex].OrdersToLineConformity[i].Orders.Count);
                    } while (secondOrderIndex == firstOrderIndex);

                    temporalOrder = populations[populationIndex].OrdersToLineConformity[i].Orders[firstOrderIndex];
                    populations[populationIndex].OrdersToLineConformity[i].Orders[firstOrderIndex] = populations[populationIndex].OrdersToLineConformity[i].Orders[secondOrderIndex];
                    populations[populationIndex].OrdersToLineConformity[i].Orders[secondOrderIndex] = temporalOrder;

                    numberOfGenParesForMutation--;
                }
            }
        }
    }
}
