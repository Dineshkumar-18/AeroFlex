using AeroFlex.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplyFly.Models
{
	public class FlightClass
	{
	  [Key]
      public int FlightclassId { get; set; }
	  public int FlightScheduleId { get; set; }
	  [Required]
	  [MaxLength(30)]
	  public string ClassName { get; set; }
	  [Required]
	  public decimal BasePrice { get; set; }
	  [Required]
	  public int TotalSeats { get; set; }
		[ForeignKey("FlightScheduleId")]
	  public FlightSchedule FlightSchedule { get; set; }
	}
}
