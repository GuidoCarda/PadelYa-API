using padelya_api.models;
using padelya_api.Models;

public class LessonEnrollment
{
    public int Id { get; set; }
    public DateTime EnrollmentDate { get; set; }

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; }

    // Pago individual por inscripción
    public Payment Payment { get; set; }
}