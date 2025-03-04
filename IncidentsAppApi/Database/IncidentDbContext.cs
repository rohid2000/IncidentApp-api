using IncidentsAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentsAppApi.Database
{
    public class IncidentDbContext(DbContextOptions<IncidentDbContext> options) : DbContext(options)
    {
        public DbSet<Incident> Incidents => Set<Incident>();
    }
}
