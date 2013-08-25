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

            var profile = new Profile();

            return View(profile);
        }

        [HttpPost]
        public ActionResult Create(Profile profile)
        {
            if (RepositoryFactory.UserRepository.Queryable.Any(x => x.Identifier == CurrentUser.Identity.Name))
            {
                Message = "You already have a profile"; //TODO: redirect to existing profile
                RedirectToAction("Index", "Home");
            }

            profile.User = new User {Identifier = CurrentUser.Identity.Name, Profile = profile};

            RepositoryFactory.ProfileRepository.EnsurePersistent(profile);


            return RedirectToAction("Landing", "Home");
        }
    }
}
