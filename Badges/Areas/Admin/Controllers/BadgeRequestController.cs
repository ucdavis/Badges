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
    }
}
