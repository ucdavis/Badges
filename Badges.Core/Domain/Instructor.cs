using FluentNHibernate.Mapping;
using System.ComponentModel.DataAnnotations;

namespace Badges.Core.Domain
{
    public class Instructor : DomainObjectGuid
    {
        public virtual string FirstName { get; set; }

        [Required]
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        
        [Required]
        public virtual string Identifier { get; set; }

        public virtual string DisplayName { get { return string.Format("{0}, {1}", LastName, FirstName); } }
    }

    public class InstructorMap : ClassMap<Instructor>
    {
        public InstructorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.FirstName);
            Map(x => x.LastName).Not.Nullable();
            Map(x => x.Email);
            Map(x => x.Identifier).Unique().Not.Nullable();
        }
    }
}
