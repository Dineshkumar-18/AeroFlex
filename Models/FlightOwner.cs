using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class FlightOwner : User
    {
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyRegistrationNumber { get; set; }
        public string CompanyPhoneNumber { get; set; }
        [Required]
        public string  CompanyEmail { get; set; }
        [Required]
        public string OperatingLicenseNumber { get; set; }
        [Required]
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public int? TotalFlightsManaged { get; set; }
        public string? SupportContact { get; set; }
        public ICollection<Airline> Airlines { get; set; }
    }
}
