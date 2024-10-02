using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class ClassPricingDto
    {
        [Required]
        public string ClassName { get; set; }
        [Required]
        public decimal BasePrice { get; set; }
        [Required]
        public int TotalSeats { get; set; }
    }
}
