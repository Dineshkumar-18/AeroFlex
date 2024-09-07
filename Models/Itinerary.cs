using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class Itinerary
    {
        [Key]
        public int ItineraryId { get; set; }
        [Required]
        public int StartAirportId { get; set; }
        [Required]
        public int EndAirportId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int TotalStops { get; set; }

        [ForeignKey("StartAirportId")]
        public virtual Airport StartAirport { get; set; }
        [ForeignKey("EndAirportId")]
        public virtual Airport EndAirport { get; set; }

        public virtual ICollection<FlightSegment> FlightSegments { get; set; }
    }
}
