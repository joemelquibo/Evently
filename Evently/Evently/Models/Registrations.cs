using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Evently.Models
{
    public class Registrations
    {
        [Key]
        public int RegistrationId { get; set; }
        [ForeignKey("EventId")]
        public required Events Event { get; set; }
        [ForeignKey("UserId")]
        public required Users User{ get; set; }
        public DateTime registration_date { get; set; }

        public RegistrationStatus Status { get; set; }
        public enum RegistrationStatus
        {
            Pending,
            Confirmed,
            Cancelled
        }

    }
}