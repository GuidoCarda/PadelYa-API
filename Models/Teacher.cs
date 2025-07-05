using padelya_api.models;

namespace padelya_api.Models
{
    public class Teacher : Person
    {
        public string Title { get; set; }
        public string Institution { get; set; }

        public List<Lesson> Lessons { get; set; }
    }
}
