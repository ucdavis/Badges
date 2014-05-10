﻿using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Repositories;
using Badges.Services;

namespace Badges.Controllers
{
    [Authorize]
    public class ExperienceController : ApplicationController
    {
        private readonly IFileService _fileService;

        public ExperienceController(IRepositoryFactory repositoryFactory, IFileService fileService) : base(repositoryFactory)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Allows student to view their work, for now just pictures
        /// TODO: allow more than just pictures
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewWorkFile(Guid id)
        {
            //TODO: There are no permissions on who can view work associated with an experience
            //TODO: Should be visible by creator or instructor if InstructorVisible is true
            // && x.Experience.Creator.Identifier == CurrentUser.Identity.Name
            // x.Experience.Instructors.Any(i => i.Identifier == CurrentUser.Identity.Name)
            var work = RepositoryFactory.SupportingWorkRepository.Queryable.SingleOrDefault(x => x.Id == id);

            if (work == null || work.ContentId == null)
            {
                return new HttpNotFoundResult();
            }

            var file = _fileService.Get(work.ContentId.Value);

            return File(file.Content, work.ContentType, work.Name);
        }

        public ActionResult DeleteWorkFile(Guid id)
        {
            var file = RepositoryFactory.SupportingWorkRepository.GetNullableById(id);

            if (file == null)
            {
                return new HttpNotFoundResult("Could not find the requested supporting work file or experience.");
            }

            var experience = file.Experience;
            experience.SupportingWorks.Remove(file);
            experience.SetModified();
            RepositoryFactory.ExperienceRepository.EnsurePersistent(experience);
            if (file.ContentId != null)
            {
                _fileService.Delete(file.ContentId.Value);
                file.ContentId = null;
            }

            return RedirectToAction("ViewExperience", "Student", new { area = string.Empty, id = file.Experience.Id });
        }
    }
}
