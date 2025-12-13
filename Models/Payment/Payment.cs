using padelya_api.Constants;
using padelya_api.Models.Tournament;
using padelya_api.Models.Ecommerce;
using System.ComponentModel.DataAnnotations.Schema;

namespace padelya_api.Models
{
  public class Payment
  {
    public int Id { get; set; }
    [Column(TypeName = "decimal(18,2)")]
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
    
    public int? OrderId { get; set; }
    public virtual padelya_api.Models.Ecommerce.Order? Order { get; set; }

    public int PersonId { get; set; }
  }
}