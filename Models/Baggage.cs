using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class Baggage
    {
        [Key]
        public int BaggageId { get; set; }
        [Required]
        public int FlightId { get; set; }
        [Required]
        public int PassengerId { get; set; }
        [Required]
        public decimal CheckInWeight { get; set; }
        [Required]
        public decimal CabinWeight { get; set; }

        // Navigation Properties
        [ForeignKey("FlightId")]
        public virtual Flight? Flight { get; set; }

        [ForeignKey("PassengerId")]
        public virtual Passenger? Passenger { get; set; }
    }
}
