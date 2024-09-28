using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class AddressDto
    {
        public string? StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        public string? State { get; set; }
        public string? Zipcode { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
