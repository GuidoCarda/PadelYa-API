using padelya_api.models;

namespace padelya_api.Models
{
    public class Player : Person
    {
        public string PreferredPosition { get; set; }

        public List<Lesson> Lessons { get; set; }
    }

}
