using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class CancellationFee
    {
        [Key]
        public int CancellationFeeId { get; set; }
        [Required]
        public int FlightScheduleId { get; set; }
        [Required]
        public decimal ChargeRate { get; set; }
        [Required]
        public decimal PlatformFee { get; set; }
        [Required]
        public DateTime ApplicableDueDate { get; set; }
        [ForeignKey("FlightScheduleId")]
        public virtual FlightSchedule FlightSchedule { get; set; }

        public virtual CancellationInfo CancellationInfo { get; set; }
    }
}
