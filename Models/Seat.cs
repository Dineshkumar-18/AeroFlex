using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{

    public enum SeatStatus
    {
        AVAILABLE = 1,
        BOOKED = 2,
        RESERVED = 3
    }
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }
        [Required]
        public int FlightScheduleId { get; set; }
        [Required]
        [MaxLength(5)]
        public string SeatNumber { get; set; }
        [Required]
        public int FlightScheduleClassId { get; set; }
        [Required]
        public int SeatTypePricingId { get; set; }
        [Required]
        public decimal SeatPrice { get; set; }
        [Required]
        public SeatStatus Status { get; set; }=SeatStatus.AVAILABLE;
        public int? BookingId { get; set; }
        public int? PassengerId { get; set; }

        [ForeignKey("FlightScheduleId")]
        public virtual FlightSchedule FlightSchedule { get; set; }

        [ForeignKey("FlightScheduleClassId")]
        public virtual FlightScheduleClass FlightScheduleClass { get; set; }    
        [ForeignKey("PassengerId")]
        public virtual Passenger Passenger { get; set; }
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        [ForeignKey("SeatTypePricingId")]
        public virtual SeatTypePricing SeatTypePricing { get; set; }

        public virtual Ticket Ticket { get; set; }

        public ICollection<CancellationInfo> CancellationInfo { get; set; }
    }
}
