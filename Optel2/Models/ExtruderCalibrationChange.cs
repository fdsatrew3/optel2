using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optel2.Models
{
    public class ExtruderCalibrationChange
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal Calibration { get; set; }
        [Display(Name="Duration, s"), Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Duration { get; set; }
        public decimal Consumption { get; set; }
        [Display(Name = "Extruder")]
        public Guid ExtruderId { get; set; }
        public Extruder Extruder { get; set; }
    }
}