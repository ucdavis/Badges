using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Badges.Core.Repositories;
using Badges.Core.Domain;
using System;
using Badges.Models.Shared;
using Badges.Models.Student;
using Badges.Services;
using UCDArch.Core.PersistanceSupport;

namespace Badges.Controllers
{ 
    [Authorize(Roles=RoleNames.Student)]
    public class StudentController : ApplicationController
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        //
        // GET: /Student/

        public StudentController(IRepositoryFactory repositoryFactory, IUserService userService, IFileService fileService) : base(repositoryFactory)
        {
            _userService = userService;
            _fileService = fileService;
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
            
            ViewBag.Name = _userService.GetCurrent().Profile.DisplayName;

            return View(experiences);
        }

        public ActionResult Feedback()
        {
            var feedback =
                RepositoryFactory.FeedbackRequestRepository.Queryable.Where(
                    x => x.Experience.Creator.Identifier == CurrentUser.Identity.Name).Fetch(x => x.Experience);

            return View(feedback.ToList());
        }

        public ActionResult AddExperience()
        {
            var model = GetEditModel(new Experience {Start = DateTime.Now, Location = "Davis, CA"});
            
            return View(model);
        }

        [HttpPost]
        public ActionResult AddExperience(Experience experience, HttpPostedFileBase coverImage)
        {
            if (ModelState.IsValid)
            {
                if (coverImage != null)
                {
                    var image = _fileService.Save(coverImage, publicAccess: true);
                    experience.CoverImageUrl = image.Uri.AbsoluteUri;
                }

                RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);

                Message = "Experience Added!";
                return RedirectToAction("ViewExperience", "Student", new {id = experience.Id});
            }

            var model = GetEditModel(experience);

            return View(model);
        }

        public ActionResult EditExperience(Guid id)
        {
            var experience =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (experience == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }

            var model = GetEditModel(experience);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditExperience(Guid id, Experience experience, HttpPostedFileBase coverImage)
        {
            var experienceToEdit =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (experienceToEdit == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }

            experienceToEdit.Instructors.Clear(); //"reset" with given instructor list
            UpdateModel(experienceToEdit, "Experience");
                
            if (ModelState.IsValid)
            {
                if (coverImage != null)
                {
                    var image = _fileService.Save(coverImage, publicAccess: true);
                    experienceToEdit.CoverImageUrl = image.Uri.AbsoluteUri;
                }
                
                RepositoryFactory.ExperienceRepository.EnsurePersistent(experienceToEdit);

                Message = "Experience Updated!";
                return RedirectToAction("ViewExperience", "Student", new {id});
            }

            var model = GetEditModel(experience);

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

            var model = new ExperienceViewModel
                {
                    Experience = experience,
                    SupportingWorks = experience.SupportingWorks.ToList(),
                    ExperienceOutcomes = experience.ExperienceOutcomes.ToList(),
                    Instructors = new MultiSelectList(RepositoryFactory.InstructorRepository.GetAll(), "Id", "DisplayName"),
                    Outcomes = new SelectList(RepositoryFactory.OutcomeRepository.GetAll(), "Id", "Name"),
                    Feedback = experience.FeedbackRequests.Where(x=>x.ResponseDate != null).ToList()
                };
            
            return View(model);
        }

        /// <summary>
        /// Add supporting work to experience given by id
        /// TODO: allow adding links as well as files
        /// TODO: maybe figure out a better place to store files
        /// </summary>
        /// <param name="id">Experience to link to this work</param>
        /// <param name="model">Information about the supporting work, links/files/etc</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSupportingWork(Guid id, SupportingWorkModel model)
        {
            var experience =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (experience == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }

            var work = new SupportingWork
                {
                    Experience = experience,
                    Description = model.Description,
                    Notes = model.Notes,
                    Type = model.Type
                };

            if (string.IsNullOrWhiteSpace(model.Url))
            {
                if (model.WorkFile != null)
                {
                    work.Name = model.WorkFile.FileName;
                    work.ContentId = _fileService.Save(model.WorkFile).Id;
                    work.ContentType = model.WorkFile.ContentType;
                }
            }
            else
            {
                work.Url = model.Url;
            }

            experience.AddSupportingWork(work);

            RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);

            return RedirectToAction("ViewExperience", "Student", new {id});
        }

        /// <summary>
        /// Out the outcome with given notes to the experience
        /// </summary>
        /// <param name="id">Experience ID</param>
        /// <param name="outcomeId">Outcome ID</param>
        /// <param name="notes">Notes</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOutcome(Guid id, Guid outcomeId, string notes)
        {
            var experience =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (experience == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }

            experience.AddOutcome(new ExperienceOutcome
                {
                    Outcome = RepositoryFactory.OutcomeRepository.GetById(outcomeId),
                    Notes = notes
                });

            RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);

            return RedirectToAction("ViewExperience", "Student", new {id});
        }

        /// <summary>
        /// Request feedback via email from instructors
        /// </summary>
        /// <param name="id">experienceID</param>
        /// <param name="message">feedback request message</param>
        /// <param name="instructors">instructors to send feedback request to</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RequestFeedback(Guid id, string message, Guid[] instructors)
        {
            var experience =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (experience == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }
            
            var instructorsToNotify =
                RepositoryFactory.InstructorRepository.Queryable.Where(x => instructors.Contains(x.Id)).ToList();

            foreach (var instructor in instructorsToNotify)
            {
                experience.AddFeedbackRequest(new FeedbackRequest {Message = message, Instructor = instructor});
            }

            if (experience.InstructorViewable == false)
            {
                experience.InstructorViewable = true;
            }

            RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);

            Message = string.Format("Feedback Requests sent successfully");

            return RedirectToAction("ViewExperience", "Student", new {id});
        }

        private ExperienceEditModel GetEditModel(Experience experience)
        {
            return new ExperienceEditModel
            {
                User = _userService.GetCurrent(),
                Experience = experience,
                Instructors = RepositoryFactory.InstructorRepository.GetAll(),
                ExperienceTypes = RepositoryFactory.ExperienceTypeRepository.GetAll()
            };
        }
    }
}