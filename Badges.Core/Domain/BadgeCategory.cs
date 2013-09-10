using FluentNHibernate.Mapping;
using System.ComponentModel.DataAnnotations;

namespace Badges.Core.Domain
{
    public class BadgeCategory : DomainObjectGuid
    {
        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual string ImageUrl { get; set; }
    }

    public class BadgeCategoryMap : ClassMap<BadgeCategory>
    {
        public BadgeCategoryMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name).Not.Nullable();
            Map(x => x.ImageUrl).Not.Nullable();
        }
    }
}