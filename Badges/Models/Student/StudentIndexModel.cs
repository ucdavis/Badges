using Badges.Core.Domain;

namespace Badges.Models.Student
{
    public class StudentIndexModel
    {
        public Experience[] Experiences { get; set; }

        public FeedbackRequest[] Feedback { get; set; }
    }
}