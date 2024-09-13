using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class FlightScheduleDTO
    {
        [Required]
        public string FlightNumber { get; set; }
        [Required]
        public string DepartureAirport { get; set; }
        [Required]
        public string ArrivalAirport { get; set; }
        [Required]
        public DateTime DepartTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
    }
}
