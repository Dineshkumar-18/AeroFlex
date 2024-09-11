using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum TravelType
    {
        DOMESTIC=1,
        INTERNATIONAL=2
    }

    public class FlightTax
    {
        [Key]
        public int FlightTaxId { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int ClassId { get; set; }
        [Required]
        [MaxLength(100)]
        public string TaxType { get; set; }
        [Required]
        public decimal TaxRate { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        public TravelType TravelType { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        [ForeignKey("ClassId")]
        public Class FlightClass { get; set; }
        [ForeignKey("CurrencyId")]
        public Currency Currency { get; set; }

    }
}
