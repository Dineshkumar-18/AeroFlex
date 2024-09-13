using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class AirlineDto
    {
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
    }
}
