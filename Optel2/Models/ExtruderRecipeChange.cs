using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optel2.Models
{
    public class ExtruderRecipeChange
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string From { get; set; }
        public string On { get; set; }
        public int Duration { get; set; }
        public decimal Consumption { get; set; }
        [Display(Name = "Extruder")]
        public Guid ExtruderId { get; set; }
        public Extruder Extruder { get; set; }
    }
}