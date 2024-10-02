using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class CountryDto
    {
        [Required]
        [MaxLength(100)]
        public string CountryName { get; set; }

        [Required]
        public string CountryCode { get; set; }

        [Required]
        public string CurrencyName { get; set; } // Accept CurrencyName instead of CurrencyId
    }

}
