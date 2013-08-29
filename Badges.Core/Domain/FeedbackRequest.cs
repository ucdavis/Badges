using System;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class FeedbackRequest : DomainObjectGuid
    {
        public FeedbackRequest()
        {
            Created = DateTime.UtcNow;
        }

        public virtual string Message { get; set; }
        public virtual DateTime Created { get; set; }

        public virtual Instructor Instructor { get; set; }
        public virtual Experience Experience { get; set; }
    }

    public class FeedbackRequestMap : ClassMap<FeedbackRequest>
    {
        public FeedbackRequestMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Message);
            Map(x => x.Created);

            References(x => x.Instructor).Not.Nullable();
            References(x => x.Experience).Not.Nullable();
        }
    }
}