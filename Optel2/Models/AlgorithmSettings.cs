using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optel2.Models
{
    public class AlgorithmSettings
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Max iterations")]
        [Range(1, int.MaxValue, ErrorMessage = "At least 1 iteration required")]
        public int MaxIterations { get; set; }
        #region GeneticAlgorithmShit
        [Display(Name = "Max GA iterations")]
        [Range(1, int.MaxValue, ErrorMessage = "At least 1 GA iteration required")]
        public int NumberOfGAiterations { get; set; }
        [Display(Name = "Populations max")]
        [Range(1, int.MaxValue, ErrorMessage = "At least 1 population required")]
        public int maxPopulation { get; set; }
        [Display(Name = "Selection max")]
        [Range(1, int.MaxValue, ErrorMessage = "At least 1 selection required")]
        public int maxSelection { get; set; }
        [Display(Name = "Mutation probability")]
        [Range(1, 100, ErrorMessage = "Mutation probability must be between 1 and 100")]
        public int mutationPropability { get; set; }
        [Display(Name = "Percent of mutable gens")]
        [Range(0, 1, ErrorMessage = "Percent of mutable gens must be between 0 and 1")]
        public decimal percentOfMutableGens { get; set; }
        [Display(Name = "Crossover probability")]
        [Range(1, 100, ErrorMessage = "Crossover probability must be between 1 and 100")]
        public int crossoverPropability { get; set; }
        #endregion
        #region BruteForceAlgorithmShit
        // ???
        #endregion
    }
}