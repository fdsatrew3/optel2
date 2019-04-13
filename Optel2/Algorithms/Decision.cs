using Algorithms;
using System.Collections.Generic;

namespace Optel2.Algorithms
{
    public class Decision
    {
        public ProductionPlan Plan; // сам план в этой точке
        public decimal FunctionValue; // значение целевой фукции
        public int Iteration;
        public Decision Next;
    }
}