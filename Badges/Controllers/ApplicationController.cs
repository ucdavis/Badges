using System.Linq;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using UCDArch.Web.Controller;

namespace Badges.Controllers
{
    public abstract class ApplicationController : SuperController
    {
        protected readonly IRepositoryFactory RepositoryFactory;
        protected User AuthenticatedUser;

        protected ApplicationController(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AuthenticatedUser = RepositoryFactory.UserRepository.Queryable.SingleOrDefault(
                    user => user.Identifier == filterContext.HttpContext.User.Identity.Name);

                ViewBag.AuthenticatedUser = AuthenticatedUser;
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}