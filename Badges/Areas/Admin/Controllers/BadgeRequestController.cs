using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Badges.Services;
using Badges.Helpers;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the BadgeRequest class
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class BadgeRequestController : ApplicationController
    {
        private readonly INotificationService _notificationService;

        public BadgeRequestController(IRepositoryFactory repositoryFactory, INotificationService notificationService) : base(repositoryFactory)
        {
            _notificationService = notificationService;
        }

        public ActionResult Index()
        {
            var pendingRequests =
                RepositoryFactory.BadgeRepository.Queryable.Where(x => x.Approved == false).Fetch(x => x.Creator);

            return View(pendingRequests);
        }

        public ActionResult Review(Guid id)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null)
            {
                return HttpNotFound();
            }
            
            badge.BadgeCriterias.ToList();

            return View(badge);
        }

        [HttpPost]
        public ActionResult Approve(Guid id)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null)
            {
                return HttpNotFound();
            }

            badge.Approved = true;

            Message = "The badge was successfully approved and can now be earned by students";
            _notificationService.Notify(badge.Creator, AuthenticatedUser, 
                "Your badge design has been approved", 
                "Congratulations, your \"" + badge.Name + "\" badge was approved!",
                ActionLinkHelper.ActionLink(Url.Action("Earn", "Badge", new { area = string.Empty, id = badge.Id }), "Earn the badge"));
                

            RepositoryFactory.BadgeRepository.EnsurePersistent(badge);

            return RedirectToAction("Index");
        }

        public ActionResult Deny(Guid id, string reason)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null)
            {
                return HttpNotFound();
            }
            
            Message = "The badge has been denied and deleted from the system";

            _notificationService.Notify(badge.Creator, AuthenticatedUser, 
                "Your badge design has been rejected", 
                "Sorry, your \"" + badge.Name + "\" badge was rejected for the following reason: " + reason,
                null);
            RepositoryFactory.BadgeRepository.Remove(badge);

            return RedirectToAction("Index");
        }
    }
}
