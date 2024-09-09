using System.ComponentModel.DataAnnotations;

namespace AeroFlex.Dtos
{
    public class AccountBase
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
