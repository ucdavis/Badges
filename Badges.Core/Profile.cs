using FluentNHibernate.Mapping;

namespace Badges.Core
{
    public class Profile : DomainObjectGuid
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string LoginId { get; set; }
    }

    public class ProfileMap : ClassMap<Profile>
    {
        public ProfileMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.LoginId);
        }
    }
}
