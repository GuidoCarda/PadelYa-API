namespace padelya_api.Models
{
    public abstract class Person
    {
        public int Id { get; set; }
        public DateTime Birthdate { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
