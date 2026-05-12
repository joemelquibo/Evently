using System.ComponentModel.DataAnnotations;

namespace Evently.Models
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "";

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = "";

        public IFormFile? ProfileImage { get; set; }
    }
}