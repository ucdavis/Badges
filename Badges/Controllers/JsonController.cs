using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Badges.Core.Repositories;
using UCDArch.Web.ActionResults;

namespace Badges.Controllers
{
    /// <summary>
    /// Controller for the Json class
    /// </summary>
    [SessionState(SessionStateBehavior.Disabled)]
    public class JsonController : ApplicationController
    {
        public JsonController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Titles()
        {
            return new JsonNetResult(RepositoryFactory.TitleRepository.Queryable.Select(x => x.Name).ToArray());
        }

        public ActionResult Organizations()
        {
            return new JsonNetResult(RepositoryFactory.OrganizationRepository.Queryable.Select(x => x.Name).ToArray());
        }
    }
}
