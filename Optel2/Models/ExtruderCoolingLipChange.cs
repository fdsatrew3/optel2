﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optel2.Models
{
    public class ExtruderCoolingLipChange
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Cooling lip"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal CoolingLip { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Duration { get; set; }
        public decimal Consumption { get; set; }
        [Display(Name = "Extruder")]
        public Guid ExtruderId { get; set; }
        public Extruder Extruder { get; set; }
    }
}