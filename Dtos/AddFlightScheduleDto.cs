using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class AddFlightScheduleDto
    {
        [Required]
        public string DepartureAirportName { get; set; }
        [Required]
        public string ArrivalAirportName { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
    }
}
