using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum Bookingstatus
    {
         CONFIRMED=1,
         PARTIALLY_CANCELLED=2,
         CANCELLED=3,
         REFUNDED=4
    }
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int FlightScheduleId { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        public int TotalPassengers { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public int FlightPricingId { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public Bookingstatus BookingStatus { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("FlightScheduleId")]
        public virtual FlightSchedule FlightSchedule { get; set; }

        [ForeignKey("FlightPricingId")]
        public virtual FlightPricing FlightPricing { get; set; }
        public ICollection<Passenger> Passengers { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Ticket> Ticket { get; set; }

        public virtual Payment Payment { get; set; }

    }
}
