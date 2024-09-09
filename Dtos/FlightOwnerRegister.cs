using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class FlightOwnerRegister : Register
    {
        public string CompanyName { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyEmail { get; set; }
        public string OperatingLicenseNumber { get; set; }
    }
}
