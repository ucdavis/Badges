using System.Collections.Generic;
using System.Web.Mvc;
using Badges.Core.Domain;

namespace Badges.Models.Shared
{
    public class ExperienceViewModel
    {
        public ExperienceViewModel()
        {
            SupportingWorks = new List<SupportingWork>();
            ExperienceOutcomes = new List<ExperienceOutcome>();
            Feedback = new List<FeedbackRequest>();
        }

        public Experience Experience { get; set; }

        public SelectList Outcomes { get; set; }

        public List<SupportingWork> SupportingWorks { get; set; }

        public IList<ExperienceOutcome> ExperienceOutcomes { get; set; }

        public MultiSelectList Instructors { get; set; }

        public FeedbackRequest Notification { get; set; }

        public List<FeedbackRequest> Feedback { get; set; }
    }
}