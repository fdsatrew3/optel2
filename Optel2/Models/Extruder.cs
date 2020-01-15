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
        [Display(Name = "Allowed min width, m"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal WidthMin { get; set; }
        [Display(Name = "Allowed max width, m"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal WidthMax { get; set; }
        [Display(Name = "Allowed min thickness, µm"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal ThicknessMin { get; set; }
        [Display(Name = "Allowed max thickness, µm"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal ThicknessMax { get; set; }
        [Display(Name = "Allowed min production speed, m/min"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal ProductionSpeedMin { get; set; }
        [Display(Name = "Allowed max production speed, m/min"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal ProductionSpeedMax { get; set; }
        public decimal DiameterMin { get; set; }
        public decimal DiameterMax { get; set; }
        public decimal WeightMin { get; set; }
        public decimal WeightMax { get; set; }
        public decimal LengthMin { get; set; }
        public decimal LengthMax { get; set; }
        [Display(Name = "Time to adjust width, s"), Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int WidthAdjustmentTime { get; set; }
        [Display(Name = "Time to change thickness, s"), Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int ChangeOfThicknessTime { get; set; }
        [Display(Name = "Startup delay, s"), Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int StartupDelay { get; set; }
        [Display(Name = "Machine hour cost, €/h"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
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