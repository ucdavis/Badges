using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Profile : DomainObjectGuid
    {
        public virtual User User { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual byte[] Image { get; set; }
    }

    public class ProfileMap : ClassMap<Profile>
    {
        public ProfileMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            HasOne(x => x.User);

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Image).CustomType("BinaryBlob");
        }
    }
}
