using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the BadgeRequest class
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class BadgeRequestController : ApplicationController
    {
        public BadgeRequestController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
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
            RepositoryFactory.BadgeRepository.EnsurePersistent(badge);

            return RedirectToAction("Index");
        }

        public ActionResult Deny(Guid id)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null)
            {
                return HttpNotFound();
            }
            
            Message = "The badge has been denied and deleted from the system";
            RepositoryFactory.BadgeRepository.Remove(badge);

            return RedirectToAction("Index");
        }
    }
}
