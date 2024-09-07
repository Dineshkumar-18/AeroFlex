using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        public string? StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        public string? State { get; set; }
        public string? Zipcode { get; set; }
        [Required]
        public string Country { get; set; }
        public virtual User User { get; set; }
    }
}
