using System.Linq;
using System.Web.Mvc;
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

            if (roles.Any(x => x.Id == Roles.Student))
            {
                return RedirectToAction("Index", "Student");
            }
            
            if (roles.Any(x=>x.Id == Roles.Instructor))
            {
                //redirect to instructor
            }
            else if (roles.Any(x=>x.Id == Roles.Administrator))
            {
                //redirect to admin
            }
            else
            {
                return new HttpUnauthorizedResult();
            }

            return View("Index");
        }
    }
}
