using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class FlightPricingDto
    {
        [Required]
        public decimal BasePrice { get; set; }
        [Required]
        public decimal? SeasonalMultiplier { get; set; }
        [Required]
        public decimal? DemandMultiplier { get; set; }
        [Required]
        public decimal? Discount { get; set; }
    }

}
