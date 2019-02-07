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
        [Display(Name = "Nozzle")]
        public decimal NozzleInsert { get; set; }
        [Display(Name = "Alternative nozzle")]
        public decimal NozzleInsertAlternative { get; set; }
        [Display(Name = "Cooling lip")]
        public decimal CoolingLip { get; set; }
        [Display(Name = "Production speed")]
        public decimal ProductionSpeed { get; set; }
        public decimal Output { get; set; }
        [Display(Name = "Calibration diameter")]
        public decimal CalibrationDiameter { get; set; }
        [Display(Name = "Extruder")]
        public Guid ExtruderId { get; set; }
        public Extruder Extruder { get; set; }
        public decimal Cost { get; set; }
    }
}