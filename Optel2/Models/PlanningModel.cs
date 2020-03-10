using Algorithms;
using Algorithms.ObjectiveFunctions;
using Optel2.Algorithms;
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
        [Display(Name = "Objective function")]
        public OptimizationCriterion Criterion { get; set; }
        public AObjectiveFunction Function { get; set; }
        public enum PlanningAlgorithm { Genetic, BruteForce, OldPlan};
        [Display(Name = "Planning algorithm")]
        public PlanningAlgorithm SelectedAlgorithm { get; set; }

        #region DecisionTree
        [Display(Name = "Generate decision tree")]
        public bool TreeRequired { get; set; }
        public List<Decision> TreeData { get; set; }
        #endregion
        #region GenericShitForAll
        [Display(Name = "Automatically calculate algorithm parameters")]
        public bool CalculateAlgorithmsSettings { get; set; }
        public int MaxIterations { get; set; }
        #endregion
        #region GeneticAlgorithmShit
        public int NumberOfGAiterations { get; set; }
        public int maxPopulation { get; set; }
        public int maxSelection { get; set; }
        public int mutationPropability { get; set; }
        public decimal percentOfMutableGens { get; set; }
        public int crossoverPropability { get; set; }
        #endregion
        #region BruteForceAlgorithmShit
        // ???
        #endregion

        public PlanningModel()
        {
            Orders = new List<Order>();
            Extruders = new List<Extruder>();
            CalculateAlgorithmsSettings = true;
        }
    }
}