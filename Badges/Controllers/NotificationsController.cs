using Badges.Core.Repositories;
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
            var notificationsList = RepositoryFactory.NotificationRepository.Queryable.Where(x => x.To.Identifier == CurrentUser.Identity.Name);

            return View(notificationsList.ToList());
        }

    }
}
