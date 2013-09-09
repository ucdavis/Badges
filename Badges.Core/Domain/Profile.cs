using System.ComponentModel.DataAnnotations;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using System;

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

        public virtual string ContentType { get; set; }
        public virtual byte[] Image { get; set; }

        public virtual string DisplayName { get { return string.Format("{0} {1}", FirstName, LastName); } }
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
            Map(x => x.ContentType);
            Map(x => x.Image).CustomType("BinaryBlob");
        }
    }
}
