using Algorithms;
using System.Collections.Generic;

namespace Optel2.Algorithms
{
    public class Decision
    {
        public enum OperationType { CreatePopulation, Mutation, Crossover, ChangePlan };
        public OperationType Operation; // наименование операции
        public int Iteration; // итерация, на которой составлялся потомок
        public ProductionPlan Plan; // сам план в этой точке
        public decimal FunctionValue; // значение целевой фукции
        public ProductionPlan Parent; // родитель 
        public Decision Next;

        public Decision FindChild(List<Decision> treeData)
        {
            int requiredIteration = this.Iteration + 1;
            for (int i = 0; i < treeData.Count; i++)
            {
                if (treeData[i].Iteration == requiredIteration && treeData[i].Parent.Equals(this.Plan))
                {
                    return treeData[i];
                }
            }
            return null;
        }
    }
}