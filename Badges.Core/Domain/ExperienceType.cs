using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class ExperienceType : DomainObjectGuid
    {
        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string Icon { get; set; }
    }

    public class ExperienceTypeMap : ClassMap<ExperienceType>
    {
        public ExperienceTypeMap()
        {
            Table("ExperienceTypes");
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name);
            Map(x => x.Icon);
        }
    }
}