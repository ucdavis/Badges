using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Outcome : DomainObjectGuid
    {
        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual string ImageUrl { get; set; }
        [Required]
        public virtual string Description { get; set; }
    }

    public class OutcomeMap : ClassMap<Outcome>
    {
        public OutcomeMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name);
            Map(x => x.ImageUrl);
            Map(x => x.Description).StringMaxLength();
        }
    }
}
