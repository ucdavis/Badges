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
                                                  .Where(x => x.Pending)
                                                  .OrderByDescending(x => x.Created);

            Notification[] recentNotifications = RepositoryFactory.NotificationRepository.Queryable
                                                  .Where(x => x.To.Identifier == CurrentUser.Identity.Name)
                                                  .OrderByDescending(x => x.Created)
                                                  .Take(15)
                                                  .ToArray();

            var model = new NotificationsPartialModel
            {
                UnreadNotificationCount = unreadNotifications.Count(),
                RecentNotifications = recentNotifications
            };

            return View(model);
        }

        // Auth: Administrators, Instructors, Students
        // Displays the entirety of the notification 
        public ActionResult View(Guid id)
        {
            var notification = RepositoryFactory.NotificationRepository.Queryable
                                                    .SingleOrDefault(x => x.Id == id);

            if (notification == null)
            {
                return new HttpNotFoundResult();
            }

            notification.Pending = false;
            RepositoryFactory.NotificationRepository.EnsurePersistent(notification);

            var model = new NotificationViewModel
            {
                Notification = notification
            };

            return View(model);
        }
    }
}
