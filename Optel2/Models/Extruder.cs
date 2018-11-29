﻿using System;
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
        public decimal LengthMin { get; set; }
        public decimal LengthMax { get; set; }
        [DisplayFormat(DataFormatString = "{0:H:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime WidthAdjustmentTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:H:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime ChangeOfThicknessTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:H:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime StartupDelay { get; set; }
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