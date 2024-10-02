using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class Airline
    {
        [Key]
        public int AirlineId { get; set; }
        [Required]
        [MaxLength(100)]
        public string AirlineName { get; set; }
        [Required]
        [MaxLength(2)]
        public string IataCode { get; set; }
        [MaxLength(50)]
        public string? Headquarters { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateOnly FoundedYear { get; set; }
        [Required]
        public string WebsiteUrl { get; set; }
        [Required]
        public string AirlineLogo { get; set; }
        [Required]
        public int FlightOwnerId { get; set; }
        [ForeignKey("FlightOwnerId")]
        public FlightOwner FlightOwner { get; set; }
        public ICollection<Flight> Flights { get; set; }
    }
}
