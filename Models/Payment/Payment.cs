using padelya_api.Constants;
using padelya_api.Models.Tournament;

namespace padelya_api.Models
{
  public class Payment
  {
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TransactionId { get; set; }
    public string PaymentMethod { get; set; }

    public PaymentType PaymentType { get; set; } // deposit, total, balance
    public PaymentStatus PaymentStatus { get; set; } // pending, approved, rejected

    // Relaciones polimórficas - solo FK, sin navegación bilateral
    public int? BookingId { get; set; }

    public int? LessonEnrollmentId { get; set; }
    public int? TournamentEnrollmentId { get; set; }
    public int PersonId { get; set; }
  }
}