using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evently.Models
{
    public class Registrations
    {
        [Key]
        public int RegistrationId { get; set; }

        // EVENT
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public required Events Event { get; set; }

        // USER
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public required Users User { get; set; }

        // REGISTRATION DATE
        public DateTime RegistrationDate { get; set; }
            = DateTime.Now;

        // ATTENDANCE
        public bool IsPresent { get; set; }
            = false;

        // STATUS
        public RegistrationStatus Status { get; set; }

        public enum RegistrationStatus
        {
            Pending,
            Confirmed,
            Cancelled
        }
    }
}