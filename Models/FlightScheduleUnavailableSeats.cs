using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Models
{
    public class FlightScheduleUnavailableSeats
    {
        [Key]
        public int UnavailableSeatsId { get; set; }
        [Required]
        public int FlightId { get; set; }
        [Required]
        public string SeatNumber { get; set; }

        [Required]
        public string ClassType { get; set; }

        [ForeignKey("FlightScheduleId")]
        public FlightSchedule FlightSchedule { get; set; }
    }
}
