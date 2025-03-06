namespace IncidentsAppApi.Models
{
    public class Incident
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Gemeld";
        public string Priority { get; set; }
    }
}
