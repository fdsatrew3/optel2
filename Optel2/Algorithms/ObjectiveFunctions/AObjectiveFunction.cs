using Optel2.Models;
using System.Collections.Generic;

namespace Algorithms.ObjectiveFunctions
{
    public abstract class AObjectiveFunction
    {
        abstract public ExecutionTimeAndCost GetExecutionTimeAndCost(Costs costs, Extruder Line, List<Order> Orders);
        abstract protected float pRetargetingTimeCalculator(Extruder line, Order previousOrder, Order newOrder);
    }
}
