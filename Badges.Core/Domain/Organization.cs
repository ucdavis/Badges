using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Organization : DomainObjectGuid
    {
        public virtual string Name { get; set; }    
    }

    public class OrganizationMap : ClassMap<Organization>
    {
        public OrganizationMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name);
        }
    }
}