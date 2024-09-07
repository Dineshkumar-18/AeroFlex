using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class FlightScheduleClass
    {
        [Key]
        public int FlightclassId { get; set; }
        [Required]
        public int FlightScheduleId { get; set; }
        [Required]
        public string ClassName { get; set; }
        [Required]
        public decimal BasePrice { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        public int TotalSeats { get; set; }
        [ForeignKey("FlightScheduleId")]
        public FlightSchedule FlightSchedule { get; set; }

        public ICollection<Seat> Seats { get; set; }    
        public ICollection<SeatTypePricing> SeatTypePricings{ get; set; }
    }
}
