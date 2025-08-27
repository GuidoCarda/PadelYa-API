namespace padelya_api.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "booking", "user", "role"
        public string? DisplayName { get; set; } // e.g., "Reservas"
        public ICollection<SimplePermission> Permissions { get; set; } = new List<SimplePermission>();
    }
}
