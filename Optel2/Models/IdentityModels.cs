using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace Optel2.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class OptelContext : DbContext
    {
        public OptelContext() : base("DefaultConnection")
        { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ExtruderCalibrationChange> ExtruderCalibrationChanges { get; set; }
        public DbSet<ExtruderCoolingLipChange> ExtruderCoolingLipChanges { get; set; }
        public DbSet<ExtruderNozzleChange> ExtruderNozzleChanges { get; set; }
        public DbSet<ExtruderRecipeChange> ExtruderRecipeChanges { get; set; }
        public DbSet<Extruder> Extruders { get; set; }
        public DbSet<FilmRecipe> FilmRecipes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<AlgorithmSettings> AlgorithmSettings { get; set; }
    }
}