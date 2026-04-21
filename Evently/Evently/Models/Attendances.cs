using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Evently.Models
{
    public class Attendances
    {
        [Key]
        public int AttendanceId { get; set; }
        [ForeignKey("EventId")]
        public int EventId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public DateTime checkInTime { get; set; }
        [ForeignKey("UserID")]
        public int VerifiedBy { get; set; }
        public AttendanceStatus Status { get; set; }
        public enum AttendanceStatus
        {
            Present,
            Absent,
        }
    }
}
