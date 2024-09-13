using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class FlightOwnerProfile
    {
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyRegistrationNumber { get; set; }
        [Required]
        public string CompanyPhoneNumber { get; set; }
        [Required]
        public string CompanyEmail { get; set; }
        [Required]
        public string OperatingLicenseNumber { get; set; }
    }
}
