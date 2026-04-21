using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Evently.Models
{
    public class Registrations
    {
        [Key]
        public int RegistrationId { get; set; }
        [ForeignKey("EventId")]
        public int EventId { get; set; }
        [ForeignKey("UserId")]
        public int UserId{ get; set; }
        public DateTime registration_date { get; set; }
        public enum RegistrationStatus
        {
            Pending,
            Confirmed,
            Cancelled
        }

    }
}