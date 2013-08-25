using System.Web.Mvc;
using Badges.Core.Repositories;

namespace Badges.Controllers
{ 
    [Authorize] //TODO: Implement roles, restrict to student role
    public class StudentController : ApplicationController
    {
        //
        // GET: /Student/

        public StudentController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            Message = "Welcome Student";   
            return View();
        }

    }
}
