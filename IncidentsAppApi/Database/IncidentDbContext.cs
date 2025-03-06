using IncidentsAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentsAppApi.Database
{
    public class IncidentDbContext(DbContextOptions<IncidentDbContext> options) : DbContext(options)
    {
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Incident>().HasData(
                new Incident
                {
                    Id = 1,
                    Description = "Poep op de stoep",
                    Status = "In behandeling",
                    Priority = "Hoog"
                },
                new Incident
                {
                    Id = 2,
                    Description = "Omgevallen boom",
                    Status = "In behandeling",
                    Priority = "Hoog"
                },
                new Incident
                {
                    Id = 3,
                    Description = "Graffiti op de muur in een tunnel",
                    Status = "Gemeld",
                    Priority = "Laag"
                });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "rsewpersad",
                    Password = "test123",
                    IsAdmin = true
                });
        }
    }
}
