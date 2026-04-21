using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
