namespace padelya_api.Models
{
    public abstract class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Category { get; set; }
    }
}
