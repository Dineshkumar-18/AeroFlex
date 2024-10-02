using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class FlightSearchDto
    {
        [Required]
        public string DepartureAirport { get; set; }
        [Required]
        public string ArrivalAirport { get; set; }
        [Required]
        public DateOnly DepartureDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        [Required]
        public string Class {  get; set; }
        [Required]
        public int TotalPassengers { get; set; }
    }
}
