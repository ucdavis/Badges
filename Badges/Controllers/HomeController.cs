using System.Linq;
using System.Web.Mvc;
using Badges.App_Start;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using UCDArch.Web.Attributes;

namespace Badges.Controllers
{
    public class HomeController : ApplicationController
    {
        public HomeController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        [HandleTransactionsManually]
        public ActionResult Index()
        {
            var user =
    RepositoryFactory.UserRepository.Queryable.SingleOrDefault(x => x.Identifier == CurrentUser.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Create", "Profile");
            }

            var roles = user.Roles.ToList();

            if (roles.Any(x => x.Id == RoleNames.Student))
            {
                return RedirectToAction("Index", "Student");
            }

            if (roles.Any(x => x.Id == RoleNames.Instructor))
            {
                return RedirectToAction("Index", "Instructor");
            }

            if (roles.Any(x => x.Id == RoleNames.Administrator))
            {
                return RedirectToAction("Index", "Landing", new { area = "Admin" });
            }
            
            return new HttpUnauthorizedResult();
        }

        [HandleTransactionsManually]
        public ActionResult Reset()
        {
            if (CurrentUser.Identity.Name == "postit")
            {
                DbInitializer.ResetDb();
                ViewBag.Message = "The database has been reset";
            }

            return RedirectToAction("Index");
        }

        [HandleTransactionsManually]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
    }
}
