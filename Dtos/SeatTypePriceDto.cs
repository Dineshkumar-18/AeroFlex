using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class SeatTypePriceDto
    {
        [Required]
        public string ClassName { get; set; }
        [Required]
        public string SeatType { get; set; }
        [Required]
        public decimal SeatTypePrice { get; set; }

    }
}
