using Algorithms;

namespace Optel2.Algorithms
{
    public class Decision
    {
        public enum OperationType { CreatePopulation, Mutation, Crossover, ChangePlan};
        public OperationType Operation; // наименование операции
        public int Iteration; // итерация, на которой составлялся потомок
        public ProductionPlan Plan; // сам план в этой точке
        public decimal FunctionValue; // значение целевой фукции
        public ProductionPlan Parent; // родитель 
    }
}