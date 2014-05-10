using System;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Notification : DomainObjectGuid
    {
        public virtual bool Pending { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual string Message { get; set; }
        public virtual User To { get; set; }
        public virtual string Title { get; set; }
        public virtual User From { get; set; }
        public virtual string ActionURL { get; set; }
    }

    public class NotificationMap : ClassMap<Notification>
    {
        public NotificationMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Pending).Not.Nullable();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Message).StringMaxLength();
            References(x => x.To, "ToUserID").Not.Nullable();
            Map(x => x.Title).StringMaxLength();
            References(x => x.From, "FromUserID").Not.Nullable();
            Map(x => x.ActionURL).Nullable();
        }
    }
}
