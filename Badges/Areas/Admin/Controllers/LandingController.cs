using System.Web.Mvc;
using Badges.Core.Domain;
using Badges.Controllers;
using Badges.Core.Repositories;

namespace Badges.Areas.Admin.Controllers
{
    [Authorize(Roles=RoleNames.Administrator)]
    public class LandingController : ApplicationController
    {
        //Admin/Landing
        public LandingController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
