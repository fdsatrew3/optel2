using Algorithms;
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
        public List<Order> Orders { get; set; }
        public List<Extruder> Extruders { get; set; }
        public OptimizationCriterion Criterion { get; set; }
        public AObjectiveFunction Function { get; set; }
        public enum PlanningAlgorithm { Genetic, BruteForce };
        [Display(Name = "Algorithm")]
        public PlanningAlgorithm SelectedAlgorithm { get; set; }

        #region GeneticAlgorithmShit
        [Display(Name = "Count of iterations")]
        public int NumberOfGAiterations { get; set; }
        [Display(Name = "Populations max")]
        public int maxPopulation { get; set; }
        [Display(Name = "Selection max")]
        public int maxSelection { get; set; }
        [Display(Name = "Mutation probability")]
        public int mutationPropability { get; set; }
        [Display(Name = "Percent of mutable gens")]
        public decimal percentOfMutableGens { get; set; }
        [Display(Name = "Crossover probability")]
        public int crossoverPropability { get; set; }
        #endregion

        #region BruteForceAlgorithmShit
        // ???
        #endregion

        public PlanningModel()
        {
            Orders = new List<Order>();
            Extruders = new List<Extruder>();
        }
    }
}