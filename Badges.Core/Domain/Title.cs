using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Title : DomainObjectGuid
    {
        public virtual string Name { get; set; }    
    }

    public class TitleMap : ClassMap<Title>
    {
        public TitleMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name);
        }
    }
}