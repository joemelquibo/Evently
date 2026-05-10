using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evently.Models
{
    public class Events
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Event name must be between 3 and 100 characters")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 10,
            ErrorMessage = "Description must be between 10 and 500 characters")]
        public string Description { get; set; }

        // ✅ Changed: DateOnly → DateTime (matches "timestamp without time zone")
        [Required(ErrorMessage = "Event date is required")]
        public DateTime EventDate { get; set; }

        // ✅ Changed: TimeOnly → TimeSpan (matches "interval")
        [Required(ErrorMessage = "Start time is required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        [StringLength(200, MinimumLength = 3,
            ErrorMessage = "Venue must be between 3 and 200 characters")]
        public string Venue { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000")]
        public int Capacity { get; set; }

        // ✅ Nullable — handles old rows with NULL UserId
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public Users? User { get; set; }

        public EventStatus Status { get; set; }

        public enum EventStatus
        {
            Scheduled,
            Ongoing,
            Completed,
            Cancelled
        }
    }
}