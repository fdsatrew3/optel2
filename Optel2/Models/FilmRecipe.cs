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
        public string ItemNumber { get; set; }
        public string Article { get; set; }
        public string Recipe { get; set; }
        public decimal Thickness { get; set; }
        public decimal NozzleInsert { get; set; }
        public decimal NozzleInsertAlternative { get; set; }
        public decimal CoolingLip { get; set; }
        public decimal ProductionSpeed { get; set; }
        public decimal Output { get; set; }
        public decimal CalibrationDiameter { get; set; }
        public Guid ExtruderId { get; set; }
    }
}