using System.Collections.Generic;
using Badges.Core.Domain;

namespace Badges.Models.Student
{
    public class ExperienceEditModel
    {
        public User User { get; set; }
        public Experience Experience { get; set; }
        public IList<ExperienceType> ExperienceTypes { get; set; }
        public IList<Instructor> Instructors { get; set; }
    }
}