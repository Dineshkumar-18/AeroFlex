using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum PaymentStatus
    {
        PENDING=1,
        SUCCESS=2,
        FAILED=3
    }
    public enum PaymentMethod
    {
        UPI=1,
        DEBITCARD=2,
        CREDITCARD=3
    }
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Required]
        public int ReferenceId { get; set; }
        [Required]
        public decimal PaidAmount { get; set; }
        public int? BalanceAmount { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        [Required]
        public PaymentStatus PaymentStatus { get; set; }
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }
}
