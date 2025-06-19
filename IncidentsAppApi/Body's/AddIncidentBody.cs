using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentsAppApi.Body_s
{
    public class AddIncidentBody
    {
        [Required]
        public string Description { get; set; }

        [EnumDataType(typeof(IncidentStatus))]
        public string Status { get; set; } = "Gemeld";

        [EnumDataType(typeof(IncidentPriority))]
        public string? Priority { get; set; } = null;
        public int? UserId { get; set; }
        public string Location { get; set; }
    }

    public enum IncidentPriority
    {
        Hoog,
        Gemiddeld,
        Laag,
    }

    public enum IncidentStatus
    {
        Gemeld,
        Geregistreerd,
        InBehandeling,
        Afgehandeld
    }
}
