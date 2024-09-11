using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum PassengerType
    {
        INFANT=1,
        CHILDREN=2,
        ADULT=3
    }

    public enum PassengerStatus
    {
        CONFIRMED=1,
        CANCELLED=2
    }
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Required]
        [MaxLength(70)]
        public string Firstname { get; set; }
		[Required]
		[MaxLength(70)]
		public string Lastname { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public PassengerType PassengerType { get; set; }
        public PassengerStatus PassengerStatus { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        public virtual Ticket Ticket { get; set; }

        public virtual  Seat Seat { get; set; }

        public virtual CancellationInfo CancellationInfo { get; set; }  
    }
}
