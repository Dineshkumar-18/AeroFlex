using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
     public class FlightSegmentDTO
      {
            public int? ItineraryId { get; set; }
            [Required]
            public int FlightScheduleId { get; set; }
            public int? StopOrder { get; set; }
            public bool? IsStop { get; set; }
      }

}
