﻿using IncidentsAppApi.Body_s;
using IncidentsAppApi.Database;
using IncidentsAppApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentsAppApi.Controllers
{
    public class IncidentReturn
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string? Priority { get; set; }
        public string Username { get; set; }
        public DateTime ReportDate { get; set; }
        public string Location { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController(MyDbContext context) : ControllerBase
    {
        private readonly MyDbContext _context = context;

        [HttpPost("getAll")]
        public async Task<ActionResult<List<IncidentReturn>>> GetAllIncidents([FromBody]AllIIncident userData)
        {
            IEnumerable<User> users = _context.Users.Where(u => u.Username == userData.username);

            if (!users.Any())
            {
                return BadRequest();
            }

            if (!users.First().IsAdmin)
            {
                return BadRequest();
            }

            var incidents = await _context.Incidents.ToListAsync();

            List<IncidentReturn> result = new List<IncidentReturn>();
            foreach(Incident incident in incidents)
            {
                User? incidentUser = _context.Users.FirstOrDefault(u => u.Id == incident.UserId);

                result.Add(new IncidentReturn()
                {
                    Id = incident.Id,
                    Description = incident.Description,
                    Status = incident.Status,
                    Priority = incident.Priority,
                    Username = incidentUser?.Username ?? "",
                    ReportDate = incident.ReportDate,
                    Location = incident.Location
                });
            }

            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<Incident>>> GetIncidentsByUserid(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                return NotFound();
            }

            var incidentsQuery = _context.Incidents.Where(incident => incident.UserId == user.Id);
            List<Incident> incidents = incidentsQuery.ToList();

            return Ok(incidents);
        }

        [HttpPost]
        public async Task<ActionResult<Incident>> AddIncident(AddIncidentBody newIncident)
        {
            if (newIncident is null)
            {
                return BadRequest();
            }

            var incident = new Incident
            {
                Description = newIncident.Description,
                Status = newIncident.Status,    
                Priority = newIncident.Priority,
                UserId = newIncident.UserId,
                ReportDate = DateTime.Now,
                Location = newIncident.Location
            };

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{id}")]//Updates one or multiple properties
        public async Task<IActionResult> UpateIncidentStatusAndPriority(int id, PatchIncidentBody patchIncidentBody)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident is null)
            {
                return BadRequest();
            }

            incident.Status = patchIncidentBody.Status;
            incident.Priority = patchIncidentBody.Priority;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncidentByid(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident is null)
            {
                return NotFound();
            }

            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
