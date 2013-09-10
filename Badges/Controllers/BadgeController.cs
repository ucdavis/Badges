using System.Collections.Generic;
using Badges.Areas.Admin.Controllers;
using Badges.Core.Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Repositories;
using Badges.Models.Badge;
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
            var model = GetBadgeAddModel();

            return View(model);
        }

        private BadgeAddModel GetBadgeAddModel()
        {
            return new BadgeAddModel
                {
                    Badge = new Badge(),
                    BadgeCategories = RepositoryFactory.BadgeCategoryRepository.GetAll()
                };
        }
    }
}
