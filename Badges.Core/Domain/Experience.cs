using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Experience : DomainObjectGuid
    {
        public Experience()
        {
            Public = true;
            ExperienceTypes = new List<ExperienceType>();
        }

        [Required]
        public virtual string Name { get; set; }

        [Required] //Brief description
        [MaxLength(140)]
        public virtual string Description { get; set; }

        public virtual string Organization { get; set; }

        [Required]
        public virtual DateTime Start { get; set; }
        
        public virtual DateTime? End { get; set; }
        
        //TODO: should this be broken into an address?
        public virtual string Location { get; set; }

        public virtual string Notes { get; set; }

        public virtual bool Public { get; set; }

        public virtual IList<ExperienceType> ExperienceTypes { get; set; }
    }

    public class ExperienceMap : ClassMap<Experience>
    {
        public ExperienceMap()
        {
            Table("Experiences");
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description).Not.Nullable();
            Map(x => x.Start).Not.Nullable();
            Map(x => x.End).Column("`End`");
            Map(x => x.Location);
            Map(x => x.Notes);

            HasMany(x => x.ExperienceTypes).Cascade.None();
        }
    }
}