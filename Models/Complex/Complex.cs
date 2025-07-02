namespace padelya_api.Models
{
    public class Complex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime OpeningTime { get; set; }
        public DateTime ClosingTime { get; set; }

        public List<Court> Courts { get; set; } = [];
    }
}
