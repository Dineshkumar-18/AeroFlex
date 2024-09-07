using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
	public class Ticket
    {
        [Key]
        public int TicketId { get; set; }
        [Required]
        public int PassengerId { get; set; }
        [Required]
        public int SeatId { get; set; }
        [Required]
        public decimal TicketPrice { get; set; }
        [Required]
        public string TicketStatus { get; set; }
        public DateTime GeneratedAt { get; set; }= DateTime.Now;
        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; }
		[ForeignKey("PassengerId")]
		public virtual Passenger Passenger { get; set; }
    }
}
