using AeroFlex.Models;
using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Response
{
    public class FlightInfoResponse
    {
        public int FlightId { get; set; }
        [MaxLength(6)]
        public string FlightNumber { get; set; }
        [Required]
        public int AirlineId { get; set; }

        public int AirlineName { get; set; }

        [MaxLength(30)]
        public string AirCraftType { get; set; }

        [Required]
        public int TotalSeatColumn { get; set; }
        [Required]
        public string FlightType { get; set; }
        [Required]
        public int DepartureAirportId { get; set; }
        [Required]
        public int ArrivalAirportId { get; set; }

        public string DepartureAirportName { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int TotalSeats { get; set; }
    }
}
