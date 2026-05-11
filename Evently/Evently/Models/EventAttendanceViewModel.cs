using Evently.Models;

namespace Evently.Models.ViewModels
{
    public class EventAttendanceViewModel
    {
        // EVENT
        public Events Event { get; set; } = null!;

        // ATTENDANCES
        public List<Attendances> Attendances { get; set; }
            = new List<Attendances>();

        // TOTAL
        public int TotalParticipants { get; set; }

        // PRESENT
        public int PresentCount { get; set; }

        // ABSENT
        public int AbsentCount { get; set; }

        // RATE
        public double AttendanceRate { get; set; }
    }
}