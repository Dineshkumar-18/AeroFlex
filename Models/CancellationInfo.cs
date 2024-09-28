using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class CancellationInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CancellationId { get; set; }
        [Required]
        public int FlightScheduleId { get; set; }                   
        [Required]
        public int SeatId { get; set; }
        [Required]
        public int PassengerId { get; set; }
        [Required]
        public int CancellationFeeId { get; set; }
        [Required]
        public decimal CancellationCharge {  get; set; }
        [Required]
        public decimal PlatformAndServiceCharge { get; set; }
        [Required]
        public decimal RefundAmount { get; set; }
        [Required]
        public DateTime CancelledTime { get; set; }
        [MaxLength(255)]
        public string CancellationReason { get; set; }

        [ForeignKey("FlightScheduleId")]
        public virtual FlightSchedule FlightSchedule { get; set; }

        [ForeignKey("SeatId")]
        public virtual Seat CancelledSeat { get; set; }
        [ForeignKey("PassengerId")]
        public virtual Passenger Passenger { get; set; }

        [ForeignKey("CancellationFeeId")]
        public virtual CancellationFee CancellationFee { get; set; }
    }
}
