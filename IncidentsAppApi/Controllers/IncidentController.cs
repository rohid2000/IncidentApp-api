using IncidentsAppApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IncidentsAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        static private List<Incident> incidents = new List<Incident>
        {
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
            },
        };

        [HttpGet]
        public ActionResult<List<Incident>> GetAllIncidents()
        {
            return Ok(incidents);
        }

        [HttpGet("{id}")]
        public ActionResult<Incident> GetIncidentById(int id)
        {
            var incident = incidents.FirstOrDefault(i => i.Id == id);
            if (incident is null)
            {
                return NotFound();
            }
            return Ok(incident);
        }

        [HttpPost]
        public ActionResult<Incident> AddIncident(Incident newIncident)
        {
            if (newIncident is null)
            {
                return BadRequest();
            }
            newIncident.Id = incidents.Max(i => i.Id) + 1;
            incidents.Add(newIncident);
            return CreatedAtAction(nameof(GetIncidentById), new { id = newIncident.Id }, newIncident);
        }

        [HttpPut("{id}")]//Updates whole object
        public IActionResult UpdateIncident(int id, Incident updatedIncident)
        {
            var incident = incidents.FirstOrDefault(i => i.Id == id);
            if (incident is null)
            {
                return NotFound();
            }

            incident.Description = updatedIncident.Description;
            incident.Status = updatedIncident.Status;
            incident.Priority = updatedIncident.Priority;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveIncidentByid(int id)
        {
            var incident = incidents.FirstOrDefault(i => i.Id == id);
            if (incident is null)
            {
                return NotFound();
            }

            incidents.Remove(incident);
            return NoContent();
        }
    }
}
