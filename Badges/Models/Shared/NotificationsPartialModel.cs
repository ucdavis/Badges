using Badges.Core.Domain;

namespace Badges.Models.Shared
{
    public class NotificationsPartialModel
    {
        public int UnreadNotificationCount { get; set; }

        public Notification[] RecentNotifications { get; set; }
    }
}