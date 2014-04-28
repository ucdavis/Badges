using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the DeleteBadge class
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class DeleteBadgeController : ApplicationController
    {
        /** Default constructor **/
        public DeleteBadgeController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        // GET: /Admin/DeleteBadge/
        /**
         * Displays a list of badges and allows the user to delete a badge from the system.
         * Before the badge is deleted, it is revoked from all users who have it.
         **/
        public ActionResult Index()
        {
            // Display list of approved badges in system, and allow the user to delete them
            var approvedBadges = RepositoryFactory.BadgeRepository.Queryable.Where(x => x.Approved == true);

            return View(approvedBadges);
        }

        // GET: /Admin/DeleteBadge/Delete/{Badge ID}
        /**
         * Deletes the badge with id {Badge ID} from the database, and revokes the badge from all students who earned it.
         **/
        public ActionResult Delete(Guid id)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);
            // HTTP 404 if no badge with this id exists
            if (badge == null) return HttpNotFound();

            // Delete all badge submissions which are for this badge
            var submissionsToDelete = RepositoryFactory.BadgeSubmissionRepository.Queryable.Where(x => x.Badge.Id.Equals(id));
            foreach (var submission in submissionsToDelete)
            {
                RepositoryFactory.BadgeSubmissionRepository.Remove(submission);
            }

            // Delete the badge itself
            RepositoryFactory.BadgeRepository.Remove(badge);

            Message = "The badge was successfully deleted and revoked from all students who earned it.";
            // TODO: Notify the badge creator and/or students who had earned the badge

            return RedirectToAction("Index");
        }
    }
}
