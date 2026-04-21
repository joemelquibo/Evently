using System.ComponentModel.DataAnnotations;


namespace Evently.Models
{
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }
        public required string RoleName { get; set; }
        public string? Description { get; set; }
    }
}
