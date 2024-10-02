using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class FlightTaxDto
    {
        [Required]
        public string CountryName { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public string TravelType { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100.")]
        public decimal TaxRate { get; set; }
    }


}
