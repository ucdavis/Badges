using System.Collections.Generic;
using Badges.Areas.Admin.Controllers;
using Badges.Core.Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Repositories;
using Badges.Models.Badge;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Badges.Controllers
{
    /// <summary>
    /// Controller for the Badge class
    /// </summary>
    [Authorize(Roles=RoleNames.Student)]
    public class BadgeController : ApplicationController
    {
        public BadgeController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Browse all badges in the system
        /// </summary>
        /// <returns></returns>
        public ActionResult Browse()
        {
            var badges = RepositoryFactory.BadgeRepository.Queryable.Where(x=>x.Approved).Fetch(x=>x.Category);

            return View(badges.ToList());
        }

        /// <summary>
        /// Shows all badges you have earned as well as badges you are working on
        /// </summary>
        /// <returns></returns>
        public ActionResult MyBadges()
        {
            var mybadges =
                RepositoryFactory.BadgeSubmissionRepository.Queryable.Where(
                    x => x.Creator.Identifier == CurrentUser.Identity.Name)
                                 .OrderByDescending(x => x.CreatedOn)
                                 .Fetch(x => x.Badge);

            return View(mybadges.ToList());
        }

        /// <summary>
        /// Create a new badge!
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var model = GetBadgeAddModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BadgeAddModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var badgeToCreate = new Badge
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                ImageUrl = model.Category.ImageUrl,
                Creator = RepositoryFactory.UserRepository.Queryable.SingleOrDefault(
                    x => x.Identifier == CurrentUser.Identity.Name),
                CreatedOn = DateTime.UtcNow
            };

            foreach (var criterion in model.Criteria.Where(criteria => !string.IsNullOrWhiteSpace(criteria)))
            {
                badgeToCreate.AddCriteria(criterion);
            }

            if (badgeToCreate.BadgeCriterias.Count == 0)
            {
                Message = "You need to add at least one criteria to create a Badge";
                return RedirectToAction("Index");
            }

            Message = "Congrats, your proposed badge has been forwarded to the proper authorities";
            RepositoryFactory.BadgeRepository.EnsurePersistent(badgeToCreate);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Earn a badge by associating experiences and work
        /// </summary>
        /// <param name="id">Badge Id</param>
        /// <returns></returns>
        public ActionResult Earn(Guid id)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null)
            {
                return HttpNotFound();
            }

            var model = new BadgeApplicationModel
                {
                    Badge = badge,
                    BadgeCriterias = badge.BadgeCriterias.ToList(),
                    Fulfillments = new List<BadgeFulfillmentViewModel>()
                };

            var existingBadgeApplication =
                RepositoryFactory.BadgeSubmissionRepository.Queryable.SingleOrDefault(
                    x => x.Badge.Id == badge.Id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (existingBadgeApplication != null)
            {
                Message = "Existing badge progress found-- you can continue associating work and experiences below";

                model.Reflection = existingBadgeApplication.Reflection;

                model.Fulfillments =
                    RepositoryFactory.BadgeFulfillmentRepository.Queryable.Where(
                        x => x.BadgeSubmission.Id == existingBadgeApplication.Id)
                                     .Select(
                                         x =>
                                         new BadgeFulfillmentViewModel
                                             {
                                                 CriteriaId = x.BadgeCriteria.Id,
                                                 Comment = x.Comment,
                                                 Details =
                                                     x.Experience == null
                                                         ? x.SupportingWork.Description
                                                         : x.Experience.Name,
                                                 WorkId = x.Experience == null ? x.SupportingWork.Id : x.Experience.Id,
                                                 WorkType = x.Experience == null ? "work" : "experience"
                                             })
                                     .ToList();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Earn(Guid id, BadgeAssociatedWorkModel[] criterion, string reflections)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null) return HttpNotFound();

            var user = RepositoryFactory.UserRepository.Queryable.Single(x => x.Identifier == CurrentUser.Identity.Name);

            var submission =
                RepositoryFactory.BadgeSubmissionRepository.Queryable.SingleOrDefault(
                    x => x.Badge.Id == badge.Id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (submission == null)
            {
                submission = new BadgeSubmission { Badge = badge, Creator = user, Approved = false };
            }
            else
            {
                submission.BadgeFulfillments.Clear();
            }

            submission.Reflection = reflections;
            AssociateWorkWithCriterion(submission, criterion);
            
            Message = "Your badge progress has been saved";
            RepositoryFactory.BadgeSubmissionRepository.EnsurePersistent(submission);

            return RedirectToAction("MyBadges");
        }

        [HttpPost]
        public ActionResult Submit(Guid id, BadgeAssociatedWorkModel[] criterion, string reflections)
        {
            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null) return HttpNotFound();

            var user = RepositoryFactory.UserRepository.Queryable.Single(x => x.Identifier == CurrentUser.Identity.Name);

            var submission =
                RepositoryFactory.BadgeSubmissionRepository.Queryable.SingleOrDefault(
                    x => x.Badge.Id == badge.Id && x.Creator.Identifier == CurrentUser.Identity.Name);

            if (submission == null)
            {
                submission = new BadgeSubmission { Badge = badge, Creator = user, Approved = false };
            }
            else
            {
                submission.BadgeFulfillments.Clear();
            }

            AssociateWorkWithCriterion(submission, criterion);
            submission.Reflection = reflections;
            submission.Submitted = true;
            submission.SubmittedOn = DateTime.UtcNow;

            Message = "Your badge submission has been forwarded for review.  You should recieve a response within 48 hours.";
            RepositoryFactory.BadgeSubmissionRepository.EnsurePersistent(submission);

            return RedirectToAction("MyBadges");
        }

        /// <summary>
        /// Returns work associated with this student, optionally filtered by a 'search' string
        /// </summary>
        /// <param name="filter">Currently filters by name of experience</param>
        /// <returns>Top 5 matching results</returns>
        public ActionResult MyExperiences(string filter)
        {
            var experienceQuery = RepositoryFactory.ExperienceRepository.Queryable
                             .Where(x => x.Creator.Identifier == CurrentUser.Identity.Name);
            
            if (!string.IsNullOrWhiteSpace(filter))
            {
                experienceQuery = experienceQuery.Where(x => x.Name == filter);
            }

            var experiences = experienceQuery
                .OrderByDescending(x => x.Created)
                .Take(5);

            return GetWorkForExperiences(experiences);
        }

        /// <summary>
        /// Returns experinces that match the work name passed in as a filter
        /// </summary>
        /// <param name="filter">Name of a specific work (or set of works)</param>
        /// <returns></returns>
        public ActionResult MyWork(string filter)
        {
            var experienceIds = RepositoryFactory.SupportingWorkRepository.Queryable
                                                 .Where(
                                                     x => x.Experience.Creator.Identifier == CurrentUser.Identity.Name)
                                                 .Where(x => x.Description == filter)
                                                 .Select(x => x.Experience.Id)
                                                 .Distinct()
                                                 .ToArray();

            var experiences = RepositoryFactory.ExperienceRepository.Queryable
                                               .Where(x => experienceIds.Contains(x.Id))
                                               .OrderByDescending(x => x.Created)
                                               .Take(5);

            return GetWorkForExperiences(experiences);
        }

        private void AssociateWorkWithCriterion(BadgeSubmission submission, IEnumerable<BadgeAssociatedWorkModel> criterion)
        {
            foreach (var criterionAssocaition in criterion)
            {
                var criteria = RepositoryFactory.BadgeCriteriaRepository.GetById(criterionAssocaition.Id);

                if (criterionAssocaition.Experience != null)
                {
                    foreach (var experience in criterionAssocaition.Experience.Distinct())
                    {
                        submission.AddFulfillment(new BadgeFulfillment
                        {
                            Comment = criterionAssocaition.Comment,
                            BadgeCriteria = criteria,
                            Experience = RepositoryFactory.ExperienceRepository.GetById(experience)
                        });
                    }
                }

                if (criterionAssocaition.Work != null)
                {
                    foreach (var work in criterionAssocaition.Work.Distinct())
                    {
                        submission.AddFulfillment(new BadgeFulfillment
                        {
                            Comment = criterionAssocaition.Comment,
                            BadgeCriteria = criteria,
                            SupportingWork = RepositoryFactory.SupportingWorkRepository.GetById(work)
                        });
                    }
                }
            }
        }

        private JsonNetResult GetWorkForExperiences(IQueryable<Experience> experiencesQuery)
        {
            var experiences = experiencesQuery.Select(exp => new {exp.Id, exp.Name, exp.Description, exp.CoverImageUrl})
                                              .ToList();

            var experienceIds = experiences.Select(x => x.Id).ToArray();
           
            var work =
                RepositoryFactory.SupportingWorkRepository.Queryable.Where(x => experienceIds.Contains(x.Experience.Id))
                                 .Select(w => new { w.Id, w.Description, experienceId = w.Experience.Id, w.Type })
                                 .ToList();

            var experiencesWithWork = from experience in experiences
                                      select
                                          new
                                          {
                                              experience.Id,
                                              experience.Name,
                                              experience.Description,
                                              experience.CoverImageUrl,
                                              Work = work.Where(x => x.experienceId == experience.Id)
                                          };

            return new JsonNetResult(experiencesWithWork);
        }

        private BadgeAddModel GetBadgeAddModel()
        {
            return new BadgeAddModel
                {
                    BadgeCategories = RepositoryFactory.BadgeCategoryRepository.GetAll()
                };
        }
    }
}
