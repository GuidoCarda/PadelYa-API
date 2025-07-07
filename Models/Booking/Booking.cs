using padelya_api.Models;

namespace padelya_api.Models
{
    public class Booking
    {

        public int Id { get; set; }
        public int CourtSlotId { get; set; }
        public CourtSlot CourtSlot { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }

        // Pagos (seña, saldo, total, etc.)
        public List<Payment> Payments { get; set; } = [];

        public string Status { get; set; } // reserved_paid, reserved_deposit
    }
}