using Evently.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Evently.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public NotificationViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var today = DateTime.Today;

            var events = await _context.Events.ToListAsync();

            var notifications = new List<NotificationItem>();

            foreach (var e in events)
            {
                var eventDate = e.EventDate.Date;
                var daysUntil = (eventDate - today).Days;

                // ── 1 day before ──
                if (daysUntil == 1)
                {
                    notifications.Add(new NotificationItem
                    {
                        EventId = e.EventId,
                        Title = "Upcoming Tomorrow",
                        Message = $"\"{e.EventName}\" is happening tomorrow at {e.Venue}.",
                        Type = "upcoming",
                        Icon = "bi-clock-fill",
                        Time = e.EventDate.ToString("MMM dd, yyyy")
                    });
                }
                // ── Event is today ──
                else if (daysUntil == 0)
                {
                    var now = DateTime.Now.TimeOfDay;
                    var hasStarted = now >= e.StartTime;
                    var hasEnded = now > e.EndTime;

                    if (!hasStarted)
                    {
                        notifications.Add(new NotificationItem
                        {
                            EventId = e.EventId,
                            Title = "Starting Today",
                            Message = $"\"{e.EventName}\" starts today at {e.StartTime:hh\\:mm} at {e.Venue}.",
                            Type = "today",
                            Icon = "bi-calendar-event-fill",
                            Time = "Today"
                        });
                    }
                    else if (hasStarted && !hasEnded)
                    {
                        notifications.Add(new NotificationItem
                        {
                            EventId = e.EventId,
                            Title = "Happening Now",
                            Message = $"\"{e.EventName}\" is ongoing right now at {e.Venue}.",
                            Type = "ongoing",
                            Icon = "bi-broadcast",
                            Time = "Now"
                        });
                    }
                    else
                    {
                        notifications.Add(new NotificationItem
                        {
                            EventId = e.EventId,
                            Title = "Event Ended",
                            Message = $"\"{e.EventName}\" has just ended.",
                            Type = "ended",
                            Icon = "bi-check-circle-fill",
                            Time = "Earlier today"
                        });
                    }
                }
                // ── Ended yesterday ──
                else if (daysUntil == -1)
                {
                    notifications.Add(new NotificationItem
                    {
                        EventId = e.EventId,
                        Title = "Ended Yesterday",
                        Message = $"\"{e.EventName}\" ended yesterday.",
                        Type = "ended",
                        Icon = "bi-check-circle-fill",
                        Time = e.EventDate.ToString("MMM dd, yyyy")
                    });
                }
            }

            // Most urgent first: ongoing → today → upcoming → ended
            var order = new[] { "ongoing", "today", "upcoming", "ended" };
            notifications = notifications
                .OrderBy(n => Array.IndexOf(order, n.Type))
                .ToList();

            return View(notifications);
        }
    }

    // ── Lightweight DTO — no DB table needed ──
    public class NotificationItem
    {
        public int EventId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Time { get; set; } = "";
    }
}