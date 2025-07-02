using IncidentsAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentsAppApi.Database
{
    public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Incident> Incidents => Set<Incident>();
        public virtual DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
