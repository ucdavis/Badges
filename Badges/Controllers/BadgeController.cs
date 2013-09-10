using Badges.Core.Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Repositories;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Badges.Controllers
{
    /// <summary>
    /// Controller for the Badge class
    /// </summary>
    [Authorize(Roles=RoleNames.Student)]
    public class BadgeController : ApplicationController
    {
        public BadgeController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create a new badge!
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
    }

    public class BadgeAddModel
    {

    }
}
