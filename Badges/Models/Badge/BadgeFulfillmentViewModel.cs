using System;

namespace Badges.Models.Badge
{
    public class BadgeFulfillmentViewModel
    {
        public Guid CriteriaId { get; set; }
        public string CriteriaDetails { get; set; }
        
        public Guid WorkId { get; set; }

        public string Details { get; set; }
        public string WorkType { get; set; }

        public string Comment { get; set; }

        public string SupportType { get; set; }

        public Guid ExperienceId { get; set; }
    }
}