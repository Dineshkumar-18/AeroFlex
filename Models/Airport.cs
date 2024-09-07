using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class Airport
    {
        [Key]
        public int AirportId { get; set; }
        [Required]
        [MaxLength(100)]
        public string AirportName { get; set; }
        [Required, MaxLength(3)]
        public string IataCode { get; set; }
        [Required]
        [MaxLength(70)]
        public string City { get; set; }
        [Required]
        public int CountryId { get; set; } 
        [Required]
        [MaxLength(30)]
        public string TimeZone { get; set; }
		public ICollection<Flight> Departures { get; set; }
		public ICollection<Flight> Arrivals { get; set; }

		public ICollection<Itinerary> StartJourney {  get; set; }
        public ICollection<Itinerary> EndJourney { get; set; }

		public ICollection<FlightSchedule> ScheduleDepartures { get; set; }
		public ICollection<FlightSchedule> ScheduleArrivals { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

	}
}
