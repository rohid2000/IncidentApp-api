using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentsAppApi.Models
{
    public class Incident
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public string Status { get; set; } = "Gemeld";
        public string? Priority { get; set; } = null;
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string Location { get; set; }
    }
}
