using System.Linq;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using System;
using Badges.Services;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the Title class
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class UserController : ApplicationController
    {
        private readonly INotificationService _notificationService;
        //
        // GET: /Admin/Title/
        public UserController(IRepositoryFactory repositoryFactory, INotificationService notificationService)
            : base(repositoryFactory)
        {
            _notificationService = notificationService;
        }

        public ActionResult Index()
        {
            var users = RepositoryFactory.UserRepository.GetAll();

            return View(users.ToList());
        }

        public ActionResult GrantInstructorPermissions(string id)
        {
            var userProfileToEdit =
                RepositoryFactory.UserRepository.Queryable.SingleOrDefault(
                    x => x.Identifier == id);

            if (userProfileToEdit == null)
            {
                Message = "This user does not exist";
                return RedirectToAction("Index");
            }

            // See if they already have instructor permissions, if so then skip
            if (userProfileToEdit.Roles.Contains(RepositoryFactory.RoleRepository.GetById(RoleNames.Instructor)))
            {
                var failModel = new InstructorPermissionResultViewModel
                {
                    Header = "This user is already an instructor.",
                    Message = "Someone has previously granted instructor permissions to " + userProfileToEdit.Profile.DisplayName + "."
                };

                return View(failModel);
            }

            userProfileToEdit.Roles.Clear();
            userProfileToEdit.Roles.Add(RepositoryFactory.RoleRepository.GetById(RoleNames.Instructor));

            RepositoryFactory.UserRepository.EnsurePersistent(userProfileToEdit);

            // Notify the user that they are now an instructor
            _notificationService.Notify(userProfileToEdit, AuthenticatedUser, 
                "You are now have instructor privileges", 
                "Congratulations! You have been verified as an instructor and have been granted elevated instructor permissions.",
                null);

            var successModel = new InstructorPermissionResultViewModel
            {
                Header = "Success!",
                Message = userProfileToEdit.Profile.DisplayName + " has been granted Instructor permissions!"
            };

            return View(successModel);
        }
    }

    public class InstructorPermissionResultViewModel
    {
        public string Header { get; set; }
        public string Message { get; set; }
    }
}
