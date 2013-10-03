using System;

namespace Badges.Models.Badge
{
    public class BadgeAssociatedWorkModel
    {
        public Guid Id { get; set; }
        public AssociatedWork[] Work { get; set; }
        
        public class AssociatedWork
        {
            public Guid? Work { get; set; }
            public Guid? Experience { get; set; }
            public string Comment { get; set; }
        }
    }
}