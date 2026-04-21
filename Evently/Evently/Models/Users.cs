using System.ComponentModel.DataAnnotations;
namespace Evently.Models
{
    public class Users
    {
        [Key]
        public int User_Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
    }
}
