using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class SeatDto
    {
        [Required]
        public string ClassName { get; set; }
    }
}
