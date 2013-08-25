using System.Collections.Generic;
using System.Web.Mvc;
using Badges.Core.Repositories;
using Badges.Core.Domain;
using System;

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

        public ActionResult AddExperience()
        {
            var model = new ExperienceEditModel
                {
                    Experience = new Experience { Start = DateTime.Now },
                    ExperienceTypes = new SelectList(RepositoryFactory.ExperienceTypeRepository.GetAll(), "Id", "Name")
                };

            return View(model);
        }
    }

    public class ExperienceEditModel
    {
        public Experience Experience { get; set; }
        public SelectList ExperienceTypes { get; set; }
    }
}
