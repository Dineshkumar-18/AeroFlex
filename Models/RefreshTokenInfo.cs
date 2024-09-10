using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class RefreshTokenInfo
    {
        [Key]
        public int RefreshTokenId { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
        [Required]
        public DateTime ExpirationTime { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
