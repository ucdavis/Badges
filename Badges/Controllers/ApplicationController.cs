using Badges.Core.Repositories;
using UCDArch.Web.Controller;

namespace Badges.Controllers
{
    public abstract class ApplicationController : SuperController
    {
        protected readonly IRepositoryFactory RepositoryFactory;

        protected ApplicationController(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }
    }
}