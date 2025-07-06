using padelya_api.models;

namespace padelya_api.Models
{
    public class Player : Person
    {
        public string PreferredPosition { get; set; }

        // Eliminada: public List<Lesson> Lessons { get; set; }
        // La relación se maneja a través de LessonEnrollment
    }

}
