using System.Web.Mvc;
using System.Web.Security;
using Badges.Core.Domain;
using UCDArch.Web.Authentication;

namespace Badges.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            string resultUrl = CASHelper.Login(); //Do the CAS Login

            if (resultUrl != null)
            {
                return Redirect(resultUrl);
            }

            TempData["URL"] = returnUrl;

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("https://cas.ucdavis.edu/cas/logout");
        }

        [Authorize(Roles = RoleNames.Administrator)]
        public ActionResult Emulate(string id /* Login ID*/)
        {
            if (!string.IsNullOrEmpty(id))
            {
                FormsAuthentication.RedirectFromLoginPage(id, false);
            }
            else
            {
                return Content("Login ID not provided.  Use /Emulate/login");
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Just a signout, without the hassle of signing out of CAS.  Ends emulated credentials.
        /// </summary>
        /// <returns></returns>
        public RedirectToRouteResult EndEmulate()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}
