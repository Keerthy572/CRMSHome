using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMSHome.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        [Required]
        public Guid BookingId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // e.g. "Success", "Pending"
        public string Status { get; set; } = "Success";

        // ✅ Store only last 4 digits
        [MaxLength(4)]
        public string CardLast4 { get; set; } = string.Empty;

        public string CardBrand { get; set; } = string.Empty;

        // ✅ Add CustomerName for admin display
        [Required]
        public string CustomerName { get; set; } = string.Empty;
    }
}
