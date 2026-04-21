using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Evently.Models
{
    public class Events
    {
        [Key]
        public int EventId { get; set; }
        public required string EventName { get; set; }  
        public required string Description { get; set; }
        public DateOnly EventDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public required string Venue { get; set; }
        public int Capacity { get; set; }
        [ForeignKey("UserId")]
        public int CreatedBy { get; set; }
<<<<<<< HEAD
=======
        public EventStatus Status { get; set; }
>>>>>>> ddf89a47745a32473fe9dc18caeebeb59841e2aa
        public enum EventStatus
        {
            Scheduled,
            Ongoing,
            Completed,
            Cancelled
        }

    }
}
