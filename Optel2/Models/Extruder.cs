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
        [Display(Name = "Allowed min width")]
        public decimal WidthMin { get; set; }
        [Display(Name = "Allowed max width")]
        public decimal WidthMax { get; set; }
        [Display(Name = "Allowed min thickness")]
        public decimal ThicknessMin { get; set; }
        [Display(Name = "Allowed max thickness")]
        public decimal ThicknessMax { get; set; }
        [Display(Name = "Allowed min production speed")]
        public decimal ProductionSpeedMin { get; set; }
        [Display(Name = "Allowed max production speed")]
        public decimal ProductionSpeedMax { get; set; }
        public decimal DiameterMin { get; set; }
        public decimal DiameterMax { get; set; }
        public decimal WeightMin { get; set; }
        public decimal WeightMax { get; set; }
        public decimal LengthMin { get; set; }
        public decimal LengthMax { get; set; }
        [Display(Name = "Time to adjust width")]
        public int WidthAdjustmentTime { get; set; }
        [Display(Name = "Time to change thickness")]
        public int ChangeOfThicknessTime { get; set; }
        [Display(Name = "Startup delay")]
        public int StartupDelay { get; set; }
        [Display(Name = "Machine hour cost")]
        public decimal MachineHourCost { get; set; }
        public decimal WidthAdjustmentConsumption { get; set; }
        public decimal ChangeOfThicknessConsumption { get; set; }
        public List<ExtruderCalibrationChange> ExtruderCalibrationChange { get; set; }
        public List<ExtruderCoolingLipChange> ExtruderCoolingLipChange { get; set; }
        public List<ExtruderNozzleChange> ExtruderNozzleChange { get; set; }
        public List<ExtruderRecipeChange> ExtruderRecipeChange { get; set; }

        [NotMapped]
        public bool Selected { get; set; }

        public Extruder()
        {
            ExtruderCalibrationChange = new List<ExtruderCalibrationChange>();
            ExtruderCoolingLipChange = new List<ExtruderCoolingLipChange>();
            ExtruderNozzleChange = new List<ExtruderNozzleChange>();
            ExtruderRecipeChange = new List<ExtruderRecipeChange>();
        }
    }
}