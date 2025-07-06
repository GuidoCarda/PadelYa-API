using padelya_api.Models.Tournament;

namespace padelya_api.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TransactionId { get; set; }

        // Relaciones polimórficas
        public int? BookingId { get; set; }
        public Booking Booking { get; set; }

        public int? LessonEnrollmentId { get; set; }
        public LessonEnrollment LessonEnrollment { get; set; }

        public int? TournamentEnrollmentId { get; set; }
        public TournamentEnrollment TournamentEnrollment { get; set; }


        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}