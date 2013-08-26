using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class ExperienceOutcome : DomainObjectGuid
    {
        public virtual int Rating { get; set; } //Something like star rating, novice->master slider
        public virtual string Notes { get; set; }
        public virtual Experience Experience { get; set; }
        public virtual Outcome Outcome { get; set; }
    }

    public class ExperienceOutcomeMap : ClassMap<ExperienceOutcome>
    {
        public ExperienceOutcomeMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Rating);
            Map(x => x.Notes);

            References(x => x.Experience).Not.Nullable();
            References(x => x.Outcome).Not.Nullable();
        }
    }
}