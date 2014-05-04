using System;
using FluentNHibernate.Mapping;
using System.Collections.Generic;

namespace Badges.Core.Domain
{
    public class Notification : DomainObjectGuid
    {
        public virtual User To { get; set; }

        public virtual bool Pending { get; set; }
        public virtual DateTime Created { get; set; }

        public virtual string Message { get; set; }
        public virtual string Subject { get; set; }

        public virtual Purpose Type { get; set; }

        public virtual Dictionary<string, string> Metadata { get; set; }

        public enum Purpose
        {
            DEFAULT,
            INSTRUCTOR_PERMISSION_REQUEST,
            BADGE_APPROVAL,
            BADGE_AWARD
        }
    }

    public class NotificationMap : ClassMap<Notification>
    {
        public NotificationMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Pending).Not.Nullable();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Message).StringMaxLength();
            Map(x => x.Subject).StringMaxLength();
            Map(x => x.Type).Not.Nullable();
            Map(x => x.Metadata).Nullable();

            References(x => x.To).Not.Nullable();
        }
    }
}
