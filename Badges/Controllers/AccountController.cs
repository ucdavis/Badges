using System.Web.Mvc;
using System.Web.Security;
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

        public ActionResult Emulate(string id /* Login ID*/)
        {
            //TODO: either remove emulate or update with roles
            if (ControllerContext.HttpContext.User.Identity.Name != "postit")
            {
                return RedirectToAction("Index", "Home"); 
            }
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
