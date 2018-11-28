﻿using Algorithms.ObjectiveFunctions;
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
        public enum SelectedAlgorithm { Genetic, BruteForce };
        public SelectedAlgorithm Algorithm { get; set; }

        #region GeneticAlgorithmShit
        [Display(Name = "Count of iterations")]
        public int NumberOfGAiterations { get; set; }
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