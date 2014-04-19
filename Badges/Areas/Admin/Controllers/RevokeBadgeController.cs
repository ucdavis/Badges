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
        
	    //private readonly IRepository<RevokeBadge> _revokeBadgeRepository;

        public RevokeBadgeController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }
    
        //
        // GET: /Admin/RevokeBadge/
        public ActionResult Index()
        {
            // Display the list of students
            var users = RepositoryFactory.UserRepository.Queryable.Where(x => x.Roles.Any(r => r.Id == RoleNames.Student));
            return View(users.ToList());
        }

        public ActionResult ViewBadges(string id)
        {
            // Display the granted badges of the given student
            var badges = RepositoryFactory.BadgeSubmissionRepository.Queryable.Where(x => x.Creator.Identifier.Equals(id))
                                 .OrderByDescending(x => x.CreatedOn).Fetch(x => x.Badge);

            return View(badges.ToList());
        }

        public ActionResult Revoke(Guid id)
        {
            var badgeSubmission = RepositoryFactory.BadgeSubmissionRepository.GetNullableById(id);

            if (badgeSubmission == null) return HttpNotFound();

            badgeSubmission.Submitted = false;
            badgeSubmission.Approved = false;

            RepositoryFactory.BadgeSubmissionRepository.EnsurePersistent(badgeSubmission);

            Message = "The badge has been revoked.";
            return RedirectToAction("Index");
        }

    }
    /*
	/// <summary>
    /// ViewModel for the RevokeBadge class
    /// </summary>
    public class RevokeBadgeViewModel
	{
		//public RevokeBadge RevokeBadge { get; set; }
 
		public static RevokeBadgeViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new RevokeBadgeViewModel {RevokeBadge = new RevokeBadge()};
 
			return viewModel;
		}
	}*/
}
