using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evently.Models
{
    public class Attendances
    {
        [Key]
        public int AttendanceId { get; set; }
        // EVENT
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Events? Event { get; set; }

        // USER
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }

        // CHECKER
        public int CheckerId { get; set; }
        [ForeignKey("CheckerId")]
        public Users? VerifiedBy { get; set; }
        // CHECK IN
        public DateTime CheckInTime { get; set; }
        // STATUS
        public AttendanceStatus Status { get; set; }
        public enum AttendanceStatus
        {
            Present,
            Absent,
        }
    }
}