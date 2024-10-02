using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace AeroFlex.Dtos
{
    public class AddFlightDto
    {
        [Required]
        public int AirlineId { get; set; }
        [Required]
        public string FlightNumber { get; set; }
        [Required]
        public string AirCraftType { get; set; }
        [Required]
        public string FlightType { get; set; }
        [Required]
        public int DepartAirport { get; set; }
        [Required]
        public int ArrivalAirport { get; set; }
        [Required]
        public int TotalSeats { get; set; }
        [Required]
        public int TotalSeatColumn { get; set; }

    }
}
