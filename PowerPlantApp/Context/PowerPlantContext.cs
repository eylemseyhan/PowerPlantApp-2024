using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PowerPlantApp.Models;

namespace PowerPlantApp.Context
{
    public class PowerPlantContext : IdentityDbContext<IdentityUser>
    {
        public PowerPlantContext(DbContextOptions<PowerPlantContext> options)
            : base(options)
        {
        }

        public DbSet<PowerPlant> PowerPlants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data eklenmesi
            modelBuilder.Entity<PowerPlant>().HasData(
                new PowerPlant
                {
                    Id = 1,
                    Name = "Solar Plant",
                    Description = "A solar power plant",
                    Type = "Solar"
                },
                new PowerPlant
                {
                    Id = 2,
                    Name = "Wind Plant",
                    Description = "A wind power plant",
                    Type = "Wind"
                }
            );
        }
    }
}