using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class CountryTax
    {
        [Key]
        public int CountryTaxId {  get; set; }
        public int CountryId { get; set; }
        public TravelType TravelType { get; set; }
        public decimal CountryTaxRate { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        ICollection<FlightPricing> FlightPricings { get; set; }
    }
}
