using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Evently.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public required string Email { get; set; }

        public required string Password { get; set; }
        //[Compare("Password", ErrorMessage = "Password do not match")]
        //public required string ConfirmPassword { get; set; }
        public required string PhoneNum { get; set; }

        [Column("balance")]
        public decimal Balance { get; set; } = 0.00m;

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [ForeignKey("RoleId")]
        public required Roles Role { get; set; } 

        public UserStatus Status { get; set; }
        public enum UserStatus
        {
            Active,
            Inactive,
            Suspended
        }

    }
}
