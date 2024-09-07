using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class FlightSegment
    {
        [Key]
        public int FlightSegmentId { get; set; }
        [Required]
        public int ItineraryId { get; set; }
        [Required]
        public int FlightScheduleId { get; set; }
        [Required]
        public int StopOrder { get; set; }
        [Required]
        public bool IsStop { get; set; }

        [ForeignKey("ItineraryId")]
        public virtual Itinerary Itinerary { get; set; }

        [ForeignKey("FlightScheduleId")]
        public virtual FlightSchedule FlightSchedule { get; set; }
    }
}
