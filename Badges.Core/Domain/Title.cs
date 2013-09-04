using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Title : DomainObjectGuid
    {
        [Required]
        [StringLength(256)]
        public virtual string Name { get; set; }    
    }

    public class TitleMap : ClassMap<Title>
    {
        public TitleMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name).Not.Nullable().Length(256);
        }
    }
}