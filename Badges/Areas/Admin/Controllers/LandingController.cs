using System.Web.Mvc;
using Badges.Core.Domain;

namespace Badges.Areas.Admin.Controllers
{
    [Authorize(Roles=RoleNames.Administrator)]
    public class LandingController : Controller
    {
        //Admin/Landing
        public ActionResult Index()
        {
            return View();
        }
    }
}
