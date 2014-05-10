using Badges.Core.Domain;

namespace Badges.Models.Shared
{
    public class NotificationsPartialModel
    {
        public int UnreadNotificationCount { get; set; }

        // Re-enable if the notification dropdown is to be reintroduced
        //public Notification[] RecentNotifications { get; set; }
    }
}