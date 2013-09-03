using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Repositories;

namespace Badges.Controllers
{
    public class ExperienceController : ApplicationController
    {
        public ExperienceController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
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

            if (work == null || work.Content == null)
            {
                return new HttpNotFoundResult();
            }
            
            return File(work.Content, work.ContentType, work.Name);
        }
    }
}
