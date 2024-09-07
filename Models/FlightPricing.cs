using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class FlightPricing
    {
        [Key]
        public int FlightPricingId { get; set; }
        [Required]
        public int FlightScheduleId { get; set; }
        [Required]
        public decimal BasePrice { get; set; }
        [Required]
        public int FlightTaxId { get; set; }
        public decimal? SeasonalMultiplier { get; set; }
        public decimal? DemandMultiplier { get; set; }
        public decimal? Discount {  get; set; }
        [Required]
        public decimal Totalprice {  get; set; }

        [ForeignKey("FlightScheduleId")]
        public virtual FlightSchedule FlightSchedule { get; set; }
        [ForeignKey("FlightTaxId")]
        public virtual FlightTax FlightTax { get; set; }

        public ICollection<Booking> Bookings { get; set; }

    }
}
