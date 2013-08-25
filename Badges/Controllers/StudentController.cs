using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Repositories;
using Badges.Core.Domain;
using System;
using Badges.Models.Student;
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
            return View();
        }

        public ActionResult Portfolio()
        {
            var experiences =
                RepositoryFactory.ExperienceRepository.Queryable.Where(
                    x => x.Creator.Identifier == CurrentUser.Identity.Name);

            return View(experiences);
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

        public ActionResult ViewExperience(Guid id)
        {
            var experience =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (experience == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }

            return View(experience);
        }
    }
}