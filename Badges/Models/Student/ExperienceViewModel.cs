using System.Collections.Generic;
using System.Web.Mvc;
using Badges.Core.Domain;

namespace Badges.Models.Student
{
    public class ExperienceViewModel
    {
        public Experience Experience { get; set; }

        public SelectList Outcomes { get; set; }

        public List<SupportingWork> SupportingWorks { get; set; }

        public IList<ExperienceOutcome> ExperienceOutcomes { get; set; }
    }
}