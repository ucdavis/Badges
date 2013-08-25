using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class User : DomainObjectGuid
    {
        public string Identifier { get; set; } //Login identifier via kerberos, openid, etc

        public Profile Profile { get; set; }
    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Identifier).Unique();

            HasOne(x => x.Profile).Cascade.All().Fetch.Join();
        }
    }
}