using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optel2.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string Product { get; set; }
        public decimal Width { get; set; }
        public decimal SetupTime { get; set; }
        public decimal ProductionTime { get; set; }
        public decimal ProductionInterruptionTime { get; set; }
        public decimal TotalTime { get; set; }
        public decimal FinishedGoods { get; set; }
        public decimal Granules { get; set; }
        public decimal Waste { get; set; }
        public decimal QuanityInRunningMeter { get; set; }
        public decimal RollWeightNet { get; set; }
        public decimal Rolls { get; set; }
    }
}