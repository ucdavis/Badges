using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    /// <summary>
    /// Represents an application that a person makes towards getting a badge
    /// </summary>
    public class BadgeSubmission : DomainObjectGuid
    {
        public BadgeSubmission()
        {
            CreatedOn = DateTime.UtcNow;
            BadgeFulfillments = new List<BadgeFulfillment>();
        }
        [Required]
        public virtual User Creator { get; set; }
        [Required]
        public virtual DateTime CreatedOn { get; set; }
        [Required]
        public virtual Badge Badge { get; set; }

        public virtual bool Approved { get; set; }
        public virtual DateTime? AwardedOn { get; set; }

        public virtual IList<BadgeFulfillment> BadgeFulfillments { get; set; }
    
        public virtual void AddFulfillment(BadgeFulfillment fulfillment)
        {
            fulfillment.BadgeSubmission = this;
            BadgeFulfillments.Add(fulfillment);
        }
    }

    public class BadgeApplicationMap : ClassMap<BadgeSubmission>
    {
        public BadgeApplicationMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.CreatedOn).Not.Nullable();
            Map(x => x.AwardedOn).Nullable();
            Map(x => x.Approved).Not.Nullable();

            References(x => x.Creator).Not.Nullable();
            References(x => x.Badge).Not.Nullable();

            HasMany(x => x.BadgeFulfillments).Inverse().Cascade.AllDeleteOrphan();
        }
    }
}