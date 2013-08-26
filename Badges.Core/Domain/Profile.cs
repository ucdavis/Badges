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

        public virtual string ContentType { get; set; }
        public virtual byte[] Image { get; set; }
    }

    public class ProfileMap : ClassMap<Profile>
    {
        public ProfileMap()
        {
            Table("Profiles");
            Id(x => x.Id).GeneratedBy.Foreign("User");

            HasOne(x => x.User).Class<User>().Cascade.All();
            
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.ContentType);
            Map(x => x.Image).CustomType("BinaryBlob");
        }
    }
}
