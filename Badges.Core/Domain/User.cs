using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class User : DomainObjectGuid
    {
        public virtual string Identifier { get; set; } //Login identifier via kerberos, openid, etc

        public virtual Profile Profile { get; set; }

        public virtual void AssociateProfile(Profile profile)
        {
            profile.User = this;
            Profile = profile;
        }
    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Identifier).Unique();

            HasOne(x => x.Profile)
                .Class<Profile>().Constrained()
                .Cascade.All()
                .Fetch.Join();
        }
    }
}