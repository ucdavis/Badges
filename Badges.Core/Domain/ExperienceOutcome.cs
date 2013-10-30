using System;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class ExperienceOutcome : DomainObjectGuid
    {
        public ExperienceOutcome()
        {
            Created = DateTime.UtcNow;
        }

        public virtual string Notes { get; set; }

        public virtual double Rating { get; set; } //Something like star rating, novice->master slider
        public virtual DateTime Created { get; set; }

        public virtual Experience Experience { get; set; }
        public virtual Outcome Outcome { get; set; }
    }

    public class ExperienceOutcomeMap : ClassMap<ExperienceOutcome>
    {
        public ExperienceOutcomeMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Rating);
            Map(x => x.Notes).StringMaxLength();
            Map(x => x.Created);

            References(x => x.Experience).Not.Nullable();
            References(x => x.Outcome).Not.Nullable();
        }
    }
}