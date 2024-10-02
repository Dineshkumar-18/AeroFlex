using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class AddFlightScheduleDto
    {
        [Required]
        public int DepartureAirportId { get; set; }
        [Required]
        public int ArrivalAirportId { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
    }
}
