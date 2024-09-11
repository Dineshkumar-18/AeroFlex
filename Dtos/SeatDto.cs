using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class SeatDto
    {
        [Required]
        public string SeatNumber { get; set; }
        [Required]
        public string ClassName { get; set; }
        [Required]
        public string SeatType { get; set; }
    }
}
