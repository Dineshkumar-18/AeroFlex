using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum ApprovalStatus
    {
        Pending=1,
        Approved=2,
        Rejected=3
    }

    public class FlightOwner : User
    {
       
        public string? CompanyName { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public string?  CompanyEmail { get; set; }
        public string? OperatingLicenseNumber { get; set; }
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public int? TotalFlightsManaged { get; set; }
        public string? SupportContact { get; set; }
        public string? ProfilePictureUrl { get; set; }
        [Required]
        public bool IsApproved { get; set; }=false;
        [Required]
        public ApprovalStatus ApprovalStatus { get; set; }=ApprovalStatus.Pending;
        [Required]
        public bool IsProfileCompleted { get; set; }=false;
        public ICollection<Airline> Airlines { get; set; }
    }
}
