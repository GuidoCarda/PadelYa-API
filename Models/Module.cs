namespace padelya_api.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "booking", "user", "role"
        public ICollection<SimplePermission> Permissions { get; set; } = new List<SimplePermission>();
    }
}
