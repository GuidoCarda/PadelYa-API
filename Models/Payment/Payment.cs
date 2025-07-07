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
        public string PaymentType { get; set; } // deposit, total

        // Relaciones polimórficas - solo FK, sin navegación bilateral
        public int? BookingId { get; set; }

        public int? LessonEnrollmentId { get; set; }
        public int? TournamentEnrollmentId { get; set; }
        public int PersonId { get; set; }
    }
}