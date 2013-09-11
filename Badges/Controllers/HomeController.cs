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
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        [HandleTransactionsManually]
        public ActionResult Reset()
        {
            DbInitializer.ResetDb();
            ViewBag.Message = "The database has been reset";

            return RedirectToAction("Index");
        }

        [HandleTransactionsManually]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        /// <summary>
        /// Landing page, redirects you to the proper dashboard for your roles
        /// </summary>
        /// <returns></returns>
        [Authorize] 
        public ActionResult Landing()
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
            
            if (roles.Any(x=>x.Id == RoleNames.Instructor))
            {
                return RedirectToAction("Index", "Instructor");
            }
            
            if (roles.Any(x=>x.Id == RoleNames.Administrator))
            {
                return RedirectToAction("Index", "Landing", new {area = "Admin"});
            }
            else
            {
                return new HttpUnauthorizedResult();
            }

            return View("Index");
        }
    }
}
