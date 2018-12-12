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

        public decimal Priority { get; set; } // Ganoes addition!!! (10.12.2018 23:03)

        [NotMapped]
        public bool Selected { get; set; }
        [NotMapped]
        internal DateTime PlanedStartDate { get; set; }
        [NotMapped]
        internal DateTime PlanedEndDate { get; set; }

        public bool CheckCompabilityWithLine(Extruder line)
        {
            return true;
        }

        // Возвращает рецепт из базы данных; в случае, если его нет, возвращает null 
        public FilmRecipe GetFilmRecipe()
        {
            OptelContext db = new OptelContext();
            List<FilmRecipe> filmRecipes = db.FilmRecipes.ToList();

            if (Product.Contains("red"))
            {
                string _newProduct = Product.Substring(0, Product.IndexOf("red") - 1);

                for (int i = 0; i < filmRecipes.Count; i++)
                {
                    if (filmRecipes[i].Article.Equals($"{Product}{Width}_red"))
                    {
                        return filmRecipes[i];
                    }
                }

            }
            else if (Product.Contains("ESDD-T"))
            {
                for (int i = 0; i < filmRecipes.Count; i++)
                {
                    if (filmRecipes[i].Article.Substring(0, 7).Equals("{Product}-") && filmRecipes[i].Article.Substring(10, 4).Equals("-{Width}"))
                    {
                        return filmRecipes[i];
                    }
                }

            }
            else
            {
                for (int i = 0; i < filmRecipes.Count; i++)
                {
                    if (filmRecipes[i].Article.Equals($"{Product}{Width}"))
                    {
                        return filmRecipes[i];
                    }
                }
            }

            return null;
        }
    }
}