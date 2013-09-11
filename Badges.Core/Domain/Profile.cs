using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Profile : DomainObjectGuid
    {
        public Profile()
        {
            
        }

        public Profile(User user)
        {
            User = user;
        }

        public virtual User User { get; set; }

        [Required]
        public virtual string FirstName { get; set; }
        
        [Required]
        public virtual string LastName { get; set; }

        [Required]
        public virtual string Email { get; set; }

        public virtual string DisplayName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public virtual string ImageUrl { get; set; }
    }

    public class ProfileMap : ClassMap<Profile>
    {
        public ProfileMap()
        {
            Table("Profiles");
            Id(x => x.Id).GeneratedBy.Foreign("User");

            HasOne(x => x.User).Class<User>().Cascade.All();
            
            Map(x => x.FirstName).Not.Nullable();
            Map(x => x.LastName).Not.Nullable();
            Map(x => x.Email).Not.Nullable();
            Map(x => x.ImageUrl).Nullable();
        }
    }
}
