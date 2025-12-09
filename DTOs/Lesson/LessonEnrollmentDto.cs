using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Lesson
{
    public class LessonEnrollmentCreateDto
    {
        [Required(ErrorMessage = "El ID de la clase es obligatorio")]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "El ID de la persona es obligatorio")]
        public int PersonId { get; set; }

        public bool RequiresPayment { get; set; } = true; // Si requiere pago o está cubierto por suscripción
    }

    public class LessonEnrollmentResponseDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string LessonDescription { get; set; } = string.Empty;
        public DateTime LessonDate { get; set; }
        public TimeOnly LessonStartTime { get; set; }
        public int PersonId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentSurname { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentCategory { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public bool IsPaid { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class LessonEnrollmentListDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentSurname { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentCategory { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentStatus { get; set; } = "Pendiente";
    }

    public class LessonEnrollmentInitPointDto
    {
        public string init_point { get; set; } = string.Empty;
    }
}

