using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Repositories;
using Badges.Models.Public;

namespace Badges.Controllers
{
    /// <summary>
    /// Controller for the Public class
    /// </summary>
    public class PublicController : ApplicationController
    {
        public PublicController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        /// <summary>
        /// Public view of a badge, with associated work and experiences
        /// </summary>
        /// <param name="id">BadgeId</param>
        /// <returns></returns>
        public ActionResult Badge(Guid id)
        {
            var badgeSubmission = RepositoryFactory.BadgeSubmissionRepository.GetNullableById(id);

            if (badgeSubmission == null) return HttpNotFound();

            var model = new BadgeViewModel
                {
                    BadgeSubmission = badgeSubmission,
                    Badge = badgeSubmission.Badge,
                    Experiences = RepositoryFactory.BadgeFulfillmentRepository.Queryable.Where(x=>x.BadgeSubmission.Id == badgeSubmission.Id).Select(x=>x.Experience).ToList(),
                    Work = RepositoryFactory.BadgeFulfillmentRepository.Queryable.Where(x=>x.BadgeSubmission.Id == badgeSubmission.Id).Select(x=>x.SupportingWork).ToList(),
                    CreatorProfile = badgeSubmission.Creator.Profile
                };

            return View(model);
        }

        /// <summary>
        /// Public view of the criteria associated with a badge
        /// </summary>
        /// <param name="id">Badge Id</param>
        /// <returns></returns>
        public ActionResult Criteria(Guid id)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null) return HttpNotFound();

            badge.BadgeCriterias.ToList();

            return View(badge);
        }
    }
}