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
using ImageResizer;
using UCDArch.Core.PersistanceSupport;

namespace Badges.Controllers
{ 
    [Authorize(Roles=RoleNames.Student)]
    public class StudentController : ApplicationController
    {
        private const int CoverPictureWidth = 1050;
        private const int CoverPictureHeight = 350;

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
            var recentExperiences = RepositoryFactory.ExperienceRepository.Queryable
                                                     .Where(x => x.Creator.Identifier == User.Identity.Name)
                                                     .OrderByDescending(x => x.Created)
                                                     .Take(5);

            var recentFeedback = RepositoryFactory.FeedbackRequestRepository.Queryable
                                                  .Where(x => x.Experience.Creator.Identifier == User.Identity.Name)
                                                  .Where(x => x.ResponseDate.HasValue)
                                                  .OrderByDescending(x => x.ResponseDate)
                                                  .Take(5);

            var model = new StudentIndexModel
                {
                    Experiences = recentExperiences.ToArray(),
                    Feedback = recentFeedback.ToArray(),
                };

            return View(model);
        }

        public ActionResult Error()
        {
            var i = 0;
            var test = 1/i;

            return Content("Error!" + test);
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
                    experience.CoverImageUrl = CropAndSave(coverImage, CoverPictureWidth, CoverPictureHeight);
                }

                RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);

                Message = "Experience Added!";
                return RedirectToAction("ViewExperience", "Student", new {id = experience.Id});
            }

            var model = GetEditModel(experience);

            return View(model);
        }

        public ActionResult DeleteExperience(Guid id)
        {
            var experience = RepositoryFactory.ExperienceRepository.GetNullableById(id);

            if (experience == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }
            experience.ExperienceOutcomes.Clear();
            var fulfillments = RepositoryFactory.BadgeFulfillmentRepository.Queryable.Where(x => x.Experience.Id.Equals(experience.Id));
            foreach (var fulfillment in fulfillments) {
                fulfillment.Experience = null;
                RepositoryFactory.BadgeFulfillmentRepository.EnsurePersistent(fulfillment);
            }
            RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);
            RepositoryFactory.ExperienceRepository.Remove(experience);
            return RedirectToAction("Portfolio", "Student");
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
                    experienceToEdit.CoverImageUrl = CropAndSave(coverImage, CoverPictureWidth, CoverPictureHeight);
                }

                experienceToEdit.LastModified = DateTime.UtcNow;
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
                    Instructors = new MultiSelectList(RepositoryFactory.InstructorRepository.Queryable.OrderBy(x=>x.LastName).ToList(), "Id", "DisplayName"),
                    Outcomes = new SelectList(RepositoryFactory.OutcomeRepository.Queryable.OrderBy(x=>x.Name), "Id", "Name"),
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
        /// Add the outcome with given notes to the experience
        /// </summary>
        /// <param name="id">Experience ID</param>
        /// <param name="outcomeId">Outcome ID</param>
        /// <param name="existingOutcomeId">If we are editing an existing outcome, this is the Id of the existing outcome</param>
        /// <param name="notes">Notes</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddEditOutcome(Guid id, Guid outcomeId, Guid? existingOutcomeId, string notes)
        {
            var experience =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (experience == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }

            if (existingOutcomeId.HasValue)
            {
                var existingOutcome = experience.ExperienceOutcomes.Single(x => x.Id == existingOutcomeId.Value);
                existingOutcome.Outcome = RepositoryFactory.OutcomeRepository.GetById(outcomeId);
                existingOutcome.Notes = notes;
            }
            else
            {
                experience.AddOutcome(new ExperienceOutcome
                    {
                        Outcome = RepositoryFactory.OutcomeRepository.GetById(outcomeId),
                        Notes = notes,
                        Created = DateTime.UtcNow
                    });
            }

            experience.SetModified();
            RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);

            return RedirectToAction("ViewExperience", "Student", new {id});
        }

        [HttpPost]
        public ActionResult RemoveOutcome(Guid id, Guid experienceOutcomeId)
        {
            var experience =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (experience == null)
            {
                return new HttpNotFoundResult("Could not find the requested experience");
            }

            var outcome = experience.ExperienceOutcomes.Single(x => x.Id == experienceOutcomeId);

            experience.ExperienceOutcomes.Remove(outcome);
            experience.SetModified();

            RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);
         
            return RedirectToAction("ViewExperience", "Student", new { id });
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

        /// <summary>
        /// Crops the image to the given size, saves, and returns the url of the final blob
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private string CropAndSave(HttpPostedFileBase image, int width, int height)
        {
            var cropResizer = new ResizeSettings(width, height, FitMode.Crop, null);

            using (var stream = new MemoryStream())
            {
                ImageBuilder.Current.Build(image.InputStream, stream, cropResizer, disposeSource: false);

                //Save cropped and original image (for possible processing later)
                var blob = _fileService.Save(image.InputStream, stream, image.ContentType, publicAccess: true);

                return blob.Uri.AbsoluteUri;
            }
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