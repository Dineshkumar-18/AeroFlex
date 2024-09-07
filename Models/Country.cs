using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string CountryName { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual Currency Currency { get; set; }

        public ICollection<FlightTax> FlightTaxes { get; set; }
        public ICollection<Airport> Airports { get; set; }
    }
}
