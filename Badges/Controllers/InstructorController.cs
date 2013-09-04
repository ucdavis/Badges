using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using Badges.Models.Shared;
using NHibernate.Linq;

namespace Badges.Controllers
{
    [Authorize(Roles=RoleNames.Instructor)]
    public class InstructorController : ApplicationController
    {
        //
        // GET: /Instructor/
        public InstructorController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            ViewBag.NotificationCount = RepositoryFactory.FeedbackRequestRepository.Queryable.Count(
                x => x.Instructor.Identifier == CurrentUser.Identity.Name && x.ResponseDate == null);

            return View();
        }

        public ActionResult MyStudents()
        {
            var experiences =
                RepositoryFactory.ExperienceRepository.Queryable.Where(
                    x => x.Instructors.Any(i => i.Identifier == CurrentUser.Identity.Name)).ToList();

            return View(experiences);
        }

        /// <summary>
        /// Show your pending notifications
        /// </summary>
        /// <returns></returns>
        public ActionResult Notifications()
        {
            var myNotifications =
                RepositoryFactory.FeedbackRequestRepository.Queryable.Where(
                    x => x.Instructor.Identifier == CurrentUser.Identity.Name && x.ResponseDate == null)
                                 .Fetch(x => x.Experience)
                                 .ThenFetch(x=>x.Creator)
                                 .ToList();

            return View(myNotifications);
        }

        /// <summary>
        /// View an experience as an instructor
        /// NOTE: Instructor must have acccess to the experience AND the experience must have instructorViewable = true
        /// </summary>
        /// <param name="id">Experience Id</param>
        /// <param name="notificationId">Notification to show along with experience</param>
        /// <returns></returns>
        public ActionResult ViewExperience(Guid id, Guid? notificationId)
        {
            var experience =
                RepositoryFactory.ExperienceRepository.Queryable.SingleOrDefault(x => x.Id == id &&
                                                                                      x.Instructors.Any(i => i.Identifier == CurrentUser.Identity.Name));

            if (experience == null)
            {
                return new HttpNotFoundResult();
            }
            if (experience.InstructorViewable == false)
            {
                return new HttpUnauthorizedResult();
            }

            var notification = notificationId.HasValue == false
                                   ? null
                                   : RepositoryFactory.FeedbackRequestRepository.Queryable.SingleOrDefault(
                                       x =>
                                       x.Id == notificationId.Value &&
                                       x.Instructor.Identifier == CurrentUser.Identity.Name);

            var model = new ExperienceViewModel
                {
                    Experience = experience,
                    Notification = notification,
                    SupportingWorks = experience.SupportingWorks.ToList(),
                    ExperienceOutcomes = experience.ExperienceOutcomes.ToList()
                };

            return View(model);
        }

        /// <summary>
        /// Respond to some feedback
        /// </summary>
        /// <param name="id">FeedbackRequestId</param>
        /// <param name="message">The feedback message</param>
        /// <returns></returns>
        public ActionResult GiveFeedback(Guid id, string message)
        {
            var request =
                RepositoryFactory.FeedbackRequestRepository.Queryable.SingleOrDefault(
                    x => x.Id == id && x.Instructor.Identifier == CurrentUser.Identity.Name);

            if (request == null)
            {
                return HttpNotFound();
            }

            request.Response = message;
            request.ResponseDate = DateTime.UtcNow;

            RepositoryFactory.FeedbackRequestRepository.EnsurePersistent(request);

            Message = "Thanks for your feedback!";

            return RedirectToAction("Notifications");
        }
    }
}
