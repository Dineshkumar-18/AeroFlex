using Microsoft.Identity.Client;

namespace AeroFlex.Dtos
{
    public class FlightOwnerProfileUpdateDto
    {
        public string? CompanyPhoneNumber { get; set; }
        public string? CompanyEmail { get; set; }
        public string? SupportContact { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int AddressId { get; set; }
    }
}
