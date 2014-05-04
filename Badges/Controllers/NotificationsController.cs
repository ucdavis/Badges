using Badges.Core.Domain;
using Badges.Core.Repositories;
using Badges.Models.Shared;
using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Badges.Controllers
{
    public class NotificationsController : ApplicationController
    {
        public NotificationsController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }
    
        // Auth: Administrators, Instructors, Students
        // Displays a list of notifications for the user
        // GET: /Notifications/
        public ActionResult Index()
        {
            var notificationsList = RepositoryFactory.NotificationRepository.Queryable
                                                    .Where(x => x.To.Identifier == CurrentUser.Identity.Name)
                                                    .OrderByDescending(x => x.Created);

            return View(notificationsList.ToList());
        }

        // Auth: Administrators, Instructors, Students
        // Displays a partial view for the navigation bar's drop down list of notifications
        // Should only be used in PartialRequests
        public ActionResult NavigationPartial()
        {
            var unreadNotifications = RepositoryFactory.NotificationRepository.Queryable
                                                  .Where(x => x.To.Identifier == CurrentUser.Identity.Name)
                                                  .OrderByDescending(x => x.Created)
                                                  .Take(15);

            Notification[] recentNotifications = null;
            if (unreadNotifications.Count() > 0)
            {
                recentNotifications = unreadNotifications.ToArray();
            }

            var model = new NotificationsPartialModel
            {
                UnreadNotificationCount = unreadNotifications.Count(),
                RecentNotifications = recentNotifications
            };

            return View(model);
        }
    }
}
