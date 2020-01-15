using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optel2.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Order number")]
        public string OrderNumber { get; set; }
        public string Product { get; set; }
        [Display(Name = "Width, m")]
        public decimal Width { get; set; }
        public decimal SetupTime { get; set; }
        public decimal ProductionTime { get; set; }
        public decimal ProductionInterruptionTime { get; set; }
        public decimal TotalTime { get; set; }
        public decimal FinishedGoods { get; set; }
        public decimal Granules { get; set; }
        public decimal Waste { get; set; }
        [Display(Name = "Quanity in running meter, m"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal QuanityInRunningMeter { get; set; }
        [Display(Name = "Weight, kg")]
        public decimal RollWeightNet { get; set; }
        public decimal Rolls { get; set; }
        [Display(Name = "Film recipe")]
        public Guid FilmRecipeId { get; set; }
        public FilmRecipe FilmRecipe { get; set; }
        [Display(Name = "PredefinedTime, s"), Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int PredefinedTime { get; set; }
        [Display(Name = "PredefinedRetargetTime, s"), Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int PredefinedRetargetTime { get; set; }
        [NotMapped]
        public bool Selected { get; set; }
        [NotMapped]
        internal DateTime PlanedStartDate { get; set; }
        [NotMapped]
        internal DateTime PlanedEndDate { get; set; }

        public bool CheckCompabilityWithLine(Extruder line)
        {
            return true;
        }
    }
}