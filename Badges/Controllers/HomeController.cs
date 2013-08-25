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

            Message = string.Format("Welcome {0}", user.Profile.FirstName);
            return View("Index");
        }
    }
}
