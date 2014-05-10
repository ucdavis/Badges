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
    /// Controller for the RevokeBadge class
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class RevokeBadgeController : ApplicationController
    {
        /** Default constructor **/
        public RevokeBadgeController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        // GET: /Admin/RevokeBadge/
        /**
         * Displays a list of students and allows the user to revoke badges from those students.
         **/
        public ActionResult Index()
        {
            // Display the list of students that have a badge
            var users = RepositoryFactory.UserRepository.Queryable.Where(x => x.Roles.Any(r => r.Id == RoleNames.Student) && 
                RepositoryFactory.BadgeSubmissionRepository.Queryable.Any(b => b.Creator.Identifier.Equals(x.Identifier)) );
            return View(users.ToList());
        }

        // GET: /Admin/RevokeBadge/ViewBadges/{username}
        /** 
         * Displays a list of badges that are already granted to the student with the given username. 
         * The user can then revoke an individual badge.
         **/
        public ActionResult ViewBadges(string id)
        {
            // HTTP 404 if no student has this ID
            if (!RepositoryFactory.UserRepository.Queryable.Any(x => x.Identifier.Equals(id) && x.Roles.Any(r => r.Id == RoleNames.Student))) return HttpNotFound();

            // Display the granted badges of the given student
            var badgesList = RepositoryFactory.BadgeSubmissionRepository.Queryable.Where(x => x.Creator.Identifier.Equals(id) && x.Approved)
                                 .OrderByDescending(x => x.CreatedOn).Fetch(x => x.Badge).ToList();

            return View(badgesList);
        }

        // GET: /Admin/RevokeBadge/Revoke/{badgeSubmission}
        /**
         * Revokes the badge previously granted by the given BadgeSubmission.
         **/
        public ActionResult Revoke(Guid id)
        {
            // Set the BadgeSubmission's state to unsubmitted and unapproved
            var badgeSubmission = RepositoryFactory.BadgeSubmissionRepository.GetNullableById(id);

            if (badgeSubmission == null) return HttpNotFound();

            RepositoryFactory.BadgeSubmissionRepository.Remove(badgeSubmission);

            // TODO: Give a notification to the user whose badge was revoked
            Message = "The badge has been revoked.";
            return RedirectToAction("Index");
        }

    }
}
