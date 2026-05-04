using System.ComponentModel.DataAnnotations;


namespace Evently.Models
{
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }
        public RoleName Role { get; set; }
        public enum RoleName
        {
            Admin,
            Organizer,
            Participant,
        }
        public string? Description { get; set; }
    }
}
