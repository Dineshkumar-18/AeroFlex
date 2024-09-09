using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class Admin : User
    {
       
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
