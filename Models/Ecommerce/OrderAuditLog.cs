using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace padelya_api.Models.Ecommerce
{
    public class OrderAuditLog
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        // UserId can be null if action is system-generated or external (e.g. webhook), 
        // but requirements say "userId: Identificador único del usuario que ejecuta la acción".
        // We will make it nullable just in case.
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public string Action { get; set; } = string.Empty; // e.g., "Create", "UpdateStatus"

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        // For status changes
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }

        // Additional details (JSON or text)
        public string? ChangeDetails { get; set; }
    }
}
