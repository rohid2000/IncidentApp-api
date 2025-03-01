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
                Status = "Gemeld",
                Priority = "Hoog"
            },
            
            new Incident
            {
                Id = 2,
                Description = "Omgevallen boom",
                Status = "Geregistreerd",
                Priority = "Hoog"
            },
        };

        [HttpGet]
        public ActionResult<List<Incident>> GetIncidents()
        {
            return Ok(incidents);
        }
    }
}
