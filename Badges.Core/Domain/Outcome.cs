using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Outcome : DomainObjectGuid
    {
        public virtual string Name { get; set; }
    }

    public class OutcomeMap : ClassMap<Outcome>
    {
        public OutcomeMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name);
        }
    }
}
