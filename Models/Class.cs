using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Models
{
    public class Class
    {
        [Key]
        public int ClassId { get; set; }
        [Required]
        [MaxLength(30)]
        public string ClassName { get; set; }
        public ICollection<FlightTax> FlightTaxes { get; set; }
    }
}
