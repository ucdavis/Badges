using System.Web.Mvc;
using Badges.Core.Domain;

namespace Badges.Models.Student
{
    public class ExperienceEditModel
    {
        public User User { get; set; }
        public Experience Experience { get; set; }
        public SelectList ExperienceTypes { get; set; }
        public MultiSelectList Instructors { get; set; }
    }
}