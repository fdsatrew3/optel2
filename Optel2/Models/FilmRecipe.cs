using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optel2.Models
{
    public class FilmRecipe
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Item number")]
        public string ItemNumber { get; set; }
        [Display(Name = "Recipe")]
        public string Article { get; set; }
        public string Recipe { get; set; }
        public decimal Thickness { get; set; }
        [Display(Name = "Nozzle"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal NozzleInsert { get; set; }
        [Display(Name = "Alternative nozzle"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal NozzleInsertAlternative { get; set; }
        [Display(Name = "Cooling lip"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal CoolingLip { get; set; }
        [Display(Name = "Production speed"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal ProductionSpeed { get; set; }
        public decimal Output { get; set; }
        [Display(Name = "Calibration diameter"), Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal CalibrationDiameter { get; set; }
        [Display(Name = "Extruder")]
        public Guid ExtruderId { get; set; }
        public Extruder Extruder { get; set; }
        public decimal Cost { get; set; }
    }
}