using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AeroFlex.Models
{
    public class FlightScheduleLayout
    {
       
            [Key]
            public int SeatLayoutId { get; set; }
            [Required]
            public int FlightScheduleId { get; set; }
            [Required]
            public int TotalColumns { get; set; }
            [Required]
            public string LayoutPattern { get; set; }  // e.g., "AC+DEFG+HJ"
            [Required]
            public string SeatTypePattern { get; set; } // e.g., "WA+AMMA+AW"
            [Required]
            public string ClassType { get; set; }  // e.g., "Economy"
            [Required]
            public int RowCount { get; set; }

            // Navigation properties
            [ForeignKey("FlightScheduleId")]
            [JsonIgnore]
            [NotMapped]
            public virtual FlightSchedule FlightSchedule { get; set; }
   
    }
}
