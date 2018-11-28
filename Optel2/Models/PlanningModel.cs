using Algorithms.ObjectiveFunctions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Algorithms.ProductionPlan;

namespace Optel2.Models
{
    public class PlanningModel
    {
        [Display(Name = "Planned start date")]
        [DataType(DataType.Date)]
        public DateTime PlannedStartDate { get; set; }
        [Display(Name = "Planned end date")]
        [DataType(DataType.Date)]
        public DateTime PlannedEndDate { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Extruder> Extruders { get; set; }
        public OptimizationCriterion Criterion { get; set; }
        public AObjectiveFunction Function { get; set; }

        #region GeneticAlgorithmShit
        public int MaxPopulation { get; set; }
        public int MaxSelection { get; set; }
        public int MutationPropability { get; set; }
        public decimal PercentOfMutableGens { get; set; }
        public int CrossoverPropability { get; set; }
        public int NumberOfGAiterations { get; set; }
        #endregion

        public PlanningModel()
        {
            Orders = new List<Order>();
            Extruders = new List<Extruder>();
        }
    }
}