namespace padelya_api.Models
{
    public class Form
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<SimplePermission> Permissions { get; set; } = new List<SimplePermission>();
    }
}
