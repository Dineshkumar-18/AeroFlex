using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class UnavailableSeats
    {
        [Key]
        public int UnavailableSeatsId { get; set; }
        [Required]
        public int FlightId { get; set; }
        [Required]
        public string SeatNumber { get; set; }

        [Required]
        public string ClassType { get; set; }

        [ForeignKey("FlightId")]
        public Flight Flight { get; set; }
    }
}
