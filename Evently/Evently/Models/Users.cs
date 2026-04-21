using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Evently.Models
{
    public class Users
    {
        [Key]
        public int User_Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNum { get; set; }
        [ForeignKey("RoleId")]
        public required Roles RoleId { get; set; }

        public required string Status {  get; set; }

    }
}
