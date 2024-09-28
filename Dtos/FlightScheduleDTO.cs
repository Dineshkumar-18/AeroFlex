using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class FlightScheduleDTO
    {

        [Required]
        public int FlightScheduleId { get; set; }
        [Required]
        public string AirlineName { get; set; }
        [Required]
        public string DepartureAirportIataCode { get; set; }
          [Required]
        public string ArrivalAirportIataCode { get; set; }
        [Required]
        public TimeOnly Duration { get; set; }

        [Required]
        public string FlightNumber { get; set; }
        [Required]
        public string DepartureAirport { get; set; }

        [Required]
        public string DepartureCity { get; set; }
        [Required]
        public string ArrivalCity { get; set; }

        [Required]
        public string ArrivalAirport { get; set; }
        [Required]
        public DateOnly DepartureDate { get; set; }
        [Required]
        public DateOnly ArrivalDate { get; set; }
        [Required]
        public TimeOnly DepartureTime { get; set; }
        [Required]
        public TimeOnly ArrivalTime { get; set; }

        [Required]
        public decimal FlightPricings { get; set; }

        [Required]
        public decimal TaxCharges { get; set; }

        public int AvailableSeatsCount { get; set; }

        public string? AirlineImagePath { get; set; }
    }
}
