using FluentNHibernate.Mapping;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;

namespace Badges.Core.Domain
{
    public class Role : DomainObjectWithTypedId<string>
    {
        public Role()
        {
            
        }

        public Role(string id)
        {
            Id = id;
        }

        [Required]
        public virtual string Name { get; set; }
    }

    public static class RoleNames
    {
        public const string Student = "S";
        public const string Instructor = "I";
        public const string Administrator = "A";
    }

    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Table("Roles");
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name).Not.Nullable();
        }
    }
}