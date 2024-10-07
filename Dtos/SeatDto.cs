using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class SeatDto
    {
        [Required]
        public List<string> ClassNames { get; set; }
    }
}
