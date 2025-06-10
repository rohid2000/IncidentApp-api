namespace IncidentsAppApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public bool IsAdmin { get; set; } = false;
        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}