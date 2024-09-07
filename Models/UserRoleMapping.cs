using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroFlex.Models
{
    public class UserRoleMapping
    {
        [Key]
        public int UserRoleMappingId {  get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [ForeignKey("UserId")]  
        public virtual User User { get; set; }
    }
}
