using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Evently.Models
{
    public class Registrations
    {
        public int registration_Id { get; set; }
        public int event_Id { get; set; }
        public int user_Id { get; set; }
        public DateTime registration_date { get; set; }
        public enum RegistrationStatus
        {
            Pending,
            Confirmed,
            Cancelled
        }

    }
}