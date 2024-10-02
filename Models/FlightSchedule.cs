using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum FlightStatus
    {
        SCHEDULING_PROCESS=0,
        SCHEDULED = 1,
        ONTIME = 2,
        DELAYED = 3,
        BOARDING = 4,
        GATE_CLOSED = 5,
        DEPARTED = 6,
        IN_AIR = 7,
        LANDED = 8,
        ARRIVED = 9,
        CANCELLED = 10,
        DIVERTED = 11
    }

    public class FlightSchedule
    {
        [Key]
        public int FlightScheduleId { get; set; }
        [Required]
        public int FlightId { get; set; }
        [Required]  
        public int DepartureAirportId { get; set; }
        [Required]
        public int ArrivalAirportId { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan Duration { get; set; }
        [Required]
        public FlightStatus FlightStatus { get; set; }=FlightStatus.SCHEDULING_PROCESS;
        [Required]
        public DateTime ScheduledAt { get; set; }=DateTime.Now;
        public DateTime? UpdatedAt {  get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<SeatTypePricing> SeatTypePricing { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<FlightScheduleClass> FlightScheduleClasses { get; set; }

        [ForeignKey("FlightId")]
        public virtual Flight Flight { get; set; }

		[ForeignKey("DepartureAirportId")]
		public virtual Airport DepartureAirport { get; set; }

		[ForeignKey("ArrivalAirportId")]
		public virtual Airport ArrivalAirport { get; set; }

        public ICollection<FlightPricing>  FlightPricing { get; set; }
        public ICollection<CancellationInfo> CancellationInfos { get; set; }
	}
}
