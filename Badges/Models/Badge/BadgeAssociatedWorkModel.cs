using System;

namespace Badges.Models.Badge
{
    public class BadgeAssociatedWorkModel
    {
        public Guid Id { get; set; }
        public Guid[] Work { get; set; }
        public Guid[] Experience { get; set; }
    }
}