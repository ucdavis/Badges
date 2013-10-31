using System.Linq;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the Title class
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class UserController : ApplicationController
    {
        //
        // GET: /Admin/Title/
        public UserController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            var users = RepositoryFactory.UserRepository.GetAll();

            return View(users.ToList());
        }
    }
}
