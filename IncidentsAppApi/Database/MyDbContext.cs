using IncidentsAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentsAppApi.Database
{
    public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
    {
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
