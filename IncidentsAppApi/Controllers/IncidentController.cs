using IncidentsAppApi.Body_s;
using IncidentsAppApi.Database;
using IncidentsAppApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentsAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController(MyDbContext context) : ControllerBase
    {
        private readonly MyDbContext _context = context;

        [HttpPost("getAll")]
        public async Task<ActionResult<List<Incident>>> GetAllIncidents([FromBody]AllIIncident userData)
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
            return Ok(await _context.Incidents.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetIncidentByUserid(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return NotFound();
            }
            return Ok(user.Incidents);
        }

        [HttpPost]
        public async Task<ActionResult<Incident>> AddIncident(Incident newIncident)
        {
            if (newIncident is null)
            {
                return BadRequest();
            }

            _context.Incidents.Add(newIncident);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]//Updates whole object
        public async Task<IActionResult> UpdateIncident(int id, Incident updatedIncident)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident is null)
            {
                return NotFound();
            }

            incident.Description = updatedIncident.Description;
            incident.Status = updatedIncident.Status;
            incident.Priority = updatedIncident.Priority;

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
