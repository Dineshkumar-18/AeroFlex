using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class Refund
    {
        [Key]
        public int RefundId { get; set; }
        [Required] 
        public int UserId { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Required]
        public decimal RefundAmount { get; set; }
        public DateTime? RefundDate { get; set; }
        public PaymentStatus RefundStatus { get; set; }=PaymentStatus.PENDING;

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }
}
