using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optel2.Models
{
    public class ExtruderNozzleChange
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public decimal Nozzle { get; set; }
        public DateTime Duration { get; set; }
        public decimal Consumption { get; set; }
        public Guid ExtruderId { get; set; }
    }
}