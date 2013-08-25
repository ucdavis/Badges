using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Domain;
using Badges.Core.Repositories;

namespace Badges.Controllers
{
    [Authorize]
    public class ProfileController : ApplicationController
    {
        public ProfileController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Create()
        {
            if (RepositoryFactory.UserRepository.Queryable.Any(x => x.Identifier == CurrentUser.Identity.Name))
            {
                Message = "You already have a profile"; //TODO: redirect to existing profile
                RedirectToAction("Landing", "Home");
            }

            var model = new ProfileEditModel
                {
                    Profile = new Profile(),
                    Roles = RepositoryFactory.RoleRepository.GetAll()
                };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Profile profile, string roles)
        {
            if (RepositoryFactory.UserRepository.Queryable.Any(x => x.Identifier == CurrentUser.Identity.Name))
            {
                Message = "You already have a profile"; //TODO: redirect to existing profile
                RedirectToAction("Index", "Home");
            }

            var user = new User {Identifier = CurrentUser.Identity.Name, Profile = profile};
            profile.User = user;

            user.Roles.Add(RepositoryFactory.RoleRepository.GetById(roles));

            RepositoryFactory.UserRepository.EnsurePersistent(user);

            return RedirectToAction("Landing", "Home");
        }
    }

    public class ProfileEditModel
    {
        public Profile Profile { get; set; }
        public IList<Role> Roles { get; set; }
    }
}
