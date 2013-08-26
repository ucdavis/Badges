using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Badges.Core.Repositories;

namespace Badges.Controllers
{
    [Authorize]
    public class InstructorController : ApplicationController
    {
        //
        // GET: /Instructor/
        public InstructorController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyStudents()
        {
            var experiences =
                RepositoryFactory.ExperienceRepository.Queryable.Where(
                    x => x.Instructors.Any(i => i.Identifier == CurrentUser.Identity.Name)).ToList();

            return View(experiences);
        }
    }
}
