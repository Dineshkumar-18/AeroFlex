using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }
        [Required]
        public string CurrencyCode { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public string Symbol { get; set; }
        [Required]
        [MaxLength(70)]
        public string CurrencyName { get; set; }
        public ICollection<FlightTax> FlightTaxes { get; set; }
        public virtual Country Country { get; set; }
    }
}
