using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public enum Roles
    {
        Admin=1,
        FlightOwner=2,
        User=3
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public int? AddressId { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        [Required]
        public DateTime RegisterationDate { get; set; }= DateTime.Now;
        public DateTime? LastLogin { get; set; }
        [Required]
        public bool IsActive { get; set; } = false;
        [Required]

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        public virtual UserRoleMapping RoleMapping { get; set; }
        public ICollection<Booking> Bookings { get; set; }

        public virtual RefreshTokenInfo RefreshTokenInfo { get; set; }
    }
}
