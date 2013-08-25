using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class User : DomainObjectGuid
    {
        public User()
        {
            Roles = new List<Role>();
        }

        public virtual string Identifier { get; set; } //Login identifier via kerberos, openid, etc

        public virtual Profile Profile { get; set; }

        public virtual IList<Role> Roles { get; set; }

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

            HasManyToMany(x => x.Roles).Table("Permissions").ParentKeyColumn("User_id").ChildKeyColumn("Role_id");
        }
    }
}