using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class Refund
    {
        [Key]
        public int RefundId { get; set; }
        [Required]
        public int CancellationId { get; set; }
        public decimal RefundAmount { get; set; }
        [Required]
        public DateTime RefundDate { get; set; }
        [Required]
        [MaxLength(255)]
        public string RefundReason { get; set; }
        public PaymentStatus RefundStatus { get; set; }

        [ForeignKey("CancellationId")]
        public virtual CancellationInfo CancellationInfo { get; set; }
    }
}
