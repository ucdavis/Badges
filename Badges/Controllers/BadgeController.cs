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
        /// Browse all badges in the system
        /// </summary>
        /// <returns></returns>
        public ActionResult Browse()
        {
            var badges = RepositoryFactory.BadgeRepository.Queryable.Fetch(x=>x.Category);

            return View(badges.ToList());
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

        [HttpPost]
        public ActionResult Create(BadgeAddModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var badgeToCreate = new Badge
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                ImageUrl = model.Category.ImageUrl,
                Creator = RepositoryFactory.UserRepository.Queryable.SingleOrDefault(
                    x => x.Identifier == CurrentUser.Identity.Name),
                CreatedOn = DateTime.UtcNow
            };

            foreach (var criterion in model.Criteria.Where(criteria => !string.IsNullOrWhiteSpace(criteria)))
            {
                badgeToCreate.AddCriteria(criterion);
            }

            if (badgeToCreate.BadgeCriterias.Count == 0)
            {
                Message = "You need to add at least one criteria to create a Badge";
                return RedirectToAction("Index");
            }

            Message = "Congrats, your proposed badge has been forwarded to the proper authorities";
            RepositoryFactory.BadgeRepository.EnsurePersistent(badgeToCreate);

            return RedirectToAction("Index");
        }

        private BadgeAddModel GetBadgeAddModel()
        {
            return new BadgeAddModel
                {
                    BadgeCategories = RepositoryFactory.BadgeCategoryRepository.GetAll()
                };
        }
    }
}
