using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum FlightType
    {
        DOMESTIC = 1,
        INTERNATIONAL = 2
    }
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }
        [Required]
        [MaxLength(6)]
        public string FlightNumber { get; set; }
        [Required]
        public int AirlineId { get; set; }
        [Required]
        [MaxLength(30)]
        public string AirCraftType { get; set; }
        [Required]
        public FlightType FlightType { get; set; }
        [Required]
        public int DepartureAirportId { get; set; }
        [Required]
        public int ArrivalAirportId { get; set; }
        [Required]
		[Range(1, int.MaxValue)]
		public int TotalSeats { get; set; }

        public ICollection<FlightSchedule> FlightSchedules { get; set; }

        [ForeignKey("AirlineId")]
        public virtual Airline Airline { get; set; }

		[ForeignKey("DepartureAirportId")]
		public virtual Airport DepartureAirport { get; set; }

		[ForeignKey("ArrivalAirportId")]
		public virtual Airport ArrivalAirport { get; set; }

	}
}
