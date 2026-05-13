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
        public string Role { get; set; } = "";
        public int EventCount { get; set; } = 0;

        public List<EventHistoryItem> EventHistory { get; set; } = new();

        public decimal Balance { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class EventHistoryItem
    {
        public string Name { get; set; } = "";
        public string Date { get; set; } = "";
        public string Status { get; set; } = "";      // "Scheduled" | "Ongoing" | "Completed"
        public string Attendance { get; set; } = "";  // "Undetermined" | "Yes" | "No"
    }
}