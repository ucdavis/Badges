using System.Collections.Generic;
using System.Web.Mvc;
using Badges.Core.Repositories;
using Badges.Core.Domain;
using System;
using Badges.Services;

namespace Badges.Controllers
{ 
    [Authorize] //TODO: Implement roles, restrict to student role
    public class StudentController : ApplicationController
    {
        private readonly IUserService _userService;
        //
        // GET: /Student/

        public StudentController(IRepositoryFactory repositoryFactory, IUserService userService) : base(repositoryFactory)
        {
            _userService = userService;
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
                    User = _userService.GetCurrent(),
                    Experience = new Experience { Start = DateTime.Now },
                    ExperienceTypes = new SelectList(RepositoryFactory.ExperienceTypeRepository.GetAll(), "Id", "Name")
                };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddExperience(Experience experience)
        {
            if (ModelState.IsValid)
            {
                RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);

                Message = "Experience Added!";
                return RedirectToAction("Index");
            }

            var model = new ExperienceEditModel
                {
                    User = _userService.GetCurrent(),
                    Experience = experience,
                    ExperienceTypes = new SelectList(RepositoryFactory.ExperienceTypeRepository.GetAll(), "Id", "Name")
                };

            return View(model);
        }
    }

    public class ExperienceEditModel
    {
        public Experience Experience { get; set; }
        public SelectList ExperienceTypes { get; set; }
        public User User { get; set; }
    }
}
