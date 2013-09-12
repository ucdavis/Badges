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
using UCDArch.Web.ActionResults;

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

        /// <summary>
        /// Earn a badge by associating experiences and work
        /// </summary>
        /// <param name="id">Badge Id</param>
        /// <returns></returns>
        public ActionResult Earn(Guid id)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null)
            {
                return HttpNotFound();
            }

            badge.BadgeCriterias.ToList(); //preload the criteria

            return View(badge);
        }

        /// <summary>
        /// Returns work associated with this student, optionally filtered by a 'search' string
        /// </summary>
        /// <param name="filter">Filters work, currently filters by name of work/experience</param>
        /// <returns>Top 5 matching results</returns>
        public ActionResult MyWork(string filter)
        {
            var experiences = RepositoryFactory.ExperienceRepository.Queryable
                             .Where(x => x.Creator.Identifier == CurrentUser.Identity.Name)
                             .OrderByDescending(x => x.Created).Take(5)
                             .Select(exp => new {exp.Id, exp.Name, exp.Description, exp.CoverImageUrl})
                             .ToList();

            var experienceIds = experiences.Select(x => x.Id).ToArray();
            var work =
                RepositoryFactory.SupportingWorkRepository.Queryable.Where(x => experienceIds.Contains(x.Experience.Id))
                                 .Select(w => new {w.Id, w.Description, experienceId = w.Experience.Id, w.Type})
                                 .ToList();

            var experiencesWithWork = from experience in experiences
                                      select
                                          new
                                              {
                                                  experience.Id,
                                                  experience.Name,
                                                  experience.Description,
                                                  experience.CoverImageUrl,
                                                  Work = work.Where(x => x.experienceId == experience.Id)
                                              };

            return new JsonNetResult(experiencesWithWork);
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
