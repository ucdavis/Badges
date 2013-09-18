using System.Collections.Generic;
using Badges.Core.Domain;

namespace Badges.Models.Public
{
    public class BadgeViewModel
    {
        public Core.Domain.Profile CreatorProfile { get; set; }
        public BadgeSubmission BadgeSubmission { get; set; }

        public Core.Domain.Badge Badge { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<SupportingWork> Work { get; set; }
    }
}