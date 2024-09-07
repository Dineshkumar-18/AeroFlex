using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum SeatType
    {
        WINDOW=1,
        MIDDLE=2,
        AISLE=3
    }
	public class SeatTypePricing
    {
        [Key]
        public int SeatTypePricingId { get; set; }
        [Required]
        public int FlightScheduleId { get; set; }
        [Required]
        public int FlightScheduleClassId { get; set; }
        [Required]
        public SeatType SeatTypeName { get; set; }
        [Required]
        public decimal SeatPriceByType { get; set; }
        public decimal TotalPriceByClassAndType { get; set; }
        [ForeignKey("FlightScheduleId")]
        public virtual FlightSchedule FlightSchedule { get; set; }
        [ForeignKey("FlightScheduleClassId ")]
        public virtual FlightScheduleClass FlightScheduleClass { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
