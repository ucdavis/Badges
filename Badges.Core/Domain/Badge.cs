using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    /// <summary>
    /// Represents an earnable badge in the system
    /// </summary>
    public class Badge : DomainObjectGuid
    {
        public Badge()
        {
            BadgeCriterias = new List<BadgeCriteria>();
            Approved = false;
        }

        [Required]
        [StringLength(64)] //Something short and descriptive
        public virtual string Name { get; set; }

        [StringLength(140)]
        public virtual string Description { get; set; }

        [Required]
        public virtual string ImageUrl { get; set; }

        [Required]
        public virtual BadgeCategory Category { get; set; }

        [Required]
        public virtual User Creator { get; set; }
        
        [Required]
        public virtual DateTime CreatedOn { get; set; }

        public virtual IList<BadgeCriteria> BadgeCriterias { get; set; }

        /// <summary>
        /// True if the badge has been approved by an admin and can be earned by others
        /// </summary>
        public virtual bool Approved { get; set; }

        public virtual void AddCriteria(string criteria)
        {
            BadgeCriterias.Add(new BadgeCriteria {Badge = this, Details = criteria});
        }
    }

    public class BadgeMap : ClassMap<Badge>
    {
        public BadgeMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name).Not.Nullable().Length(64);
            Map(x => x.Description).Length(140);
            Map(x => x.ImageUrl).Not.Nullable();
            Map(x => x.CreatedOn).Not.Nullable();
            Map(x => x.Approved).Not.Nullable();

            References(x => x.Creator).Not.Nullable();
            References(x => x.Category).Not.Nullable();

            HasMany(x => x.BadgeCriterias).Inverse().Cascade.AllDeleteOrphan();
        }
    }
}
