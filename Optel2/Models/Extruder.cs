using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optel2.Models
{
    public class Extruder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal WidthMin { get; set; }
        public decimal WidthMax { get; set; }
        public decimal ThicknessMin { get; set; }
        public decimal ThicknessMax { get; set; }
        public decimal ProductionSpeedMin { get; set; }
        public decimal ProductionSpeedMax { get; set; }
        public decimal DiameterMin { get; set; }
        public decimal DiameterMax { get; set; }
        public decimal WeightMin { get; set; }
        public decimal WeightMax { get; set; }
        public decimal LenghtMin { get; set; }
        public decimal LenghtMax { get; set; }
        public DateTime WidthAdjustmentTime { get; set; }
        public DateTime ChangeOfThicknessTime { get; set; }
        public DateTime StartupDelay { get; set; }
        public decimal MachineHourCost { get; set; }
        public decimal WidthAdjustmentConsumption { get; set; }
        public decimal ChangeOfThicknessTimeConsumption { get; set; }
        public ICollection<ExtruderCalibrationChange> CalibrationChanges { get; set; }
        public ICollection<ExtruderCoolingLipChange> CoolingLipChanges { get; set; }
        public ICollection<ExtruderNozzleChange> NozzleChanges { get; set; }
        public ICollection<ExtruderRecipeChange> RecipeChanges { get; set; }

        public Extruder()
        {
            CalibrationChanges = new List<ExtruderCalibrationChange>();
            CoolingLipChanges = new List<ExtruderCoolingLipChange>();
            NozzleChanges = new List<ExtruderNozzleChange>();
            RecipeChanges = new List<ExtruderRecipeChange>();
        }
    }
}