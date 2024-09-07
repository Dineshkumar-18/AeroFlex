using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class CancellationInfo
    {
        [Key]
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
        public int RefundAmount { get; set; }
        [Required]
        public DateTime CancelledTime { get; set; }
        [ForeignKey("FlightScheduleId")]
        public virtual FlightSchedule FlightSchedule { get; set; }

        [ForeignKey("SeatId")]
        public virtual Seat CancelledSeat { get; set; }
        [ForeignKey("PassengerId")]
        public virtual Passenger Passenger { get; set; }

        [ForeignKey("CancellationFeeId")]
        public virtual CancellationFee CancellationFee { get; set; }

        public virtual Refund Refund { get; set; }
    }
}
