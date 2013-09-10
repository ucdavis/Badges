using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    /// <summary>
    /// Specific criteria for badge, must be fulfilled later to earn this badge
    /// </summary>
    public class BadgeCriteria : DomainObjectGuid
    {
        [Required]
        public virtual Badge Badge { get; set; }

        [Required]
        [StringLength(140)]
        public virtual string Details { get; set; }
    }

    public class BadgeCriteriaMap : ClassMap<BadgeCriteria>
    {
        public BadgeCriteriaMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Details).Not.Nullable().Length(140);

            References(x => x.Badge).Not.Nullable();
        }
    }
}