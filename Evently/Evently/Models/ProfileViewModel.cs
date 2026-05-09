using System.Collections.Generic;

namespace Evently.Models
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Gender { get; set; } = "N/A";   // not in Users model; kept for display
        public string Address { get; set; } = "N/A";  // not in Users model; kept for display
        public string Role { get; set; } = "";
        public int EventCount { get; set; } = 0;

        public List<EventHistoryItem> EventHistory { get; set; } = new();
    }

    public class EventHistoryItem
    {
        public string Name { get; set; } = "";
        public string Date { get; set; } = "";
        public string Status { get; set; } = "";      // "Scheduled" | "Ongoing" | "Completed"
        public string Attendance { get; set; } = "";  // "Undetermined" | "Yes" | "No"
    }
}