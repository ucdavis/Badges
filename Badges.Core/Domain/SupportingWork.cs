using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    /// <summary>
    /// Supporting work for an experience
    /// Can be photo or file (in the future, audio & video as well)
    /// </summary>
    public class SupportingWork : DomainObjectGuid
    {
        [Required]
        public virtual string Description { get; set; }

        public virtual string Name { get; set; }
        public virtual string ContentType { get; set; }
        public virtual byte[] Content { get; set; }

        [Required]
        public virtual Experience Experience { get; set; }
    }

    public class SupportingWorkMap : ClassMap<SupportingWork>
    {
        public SupportingWorkMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name);
            Map(x => x.ContentType);
            Map(x => x.Description).Not.Nullable();
            Map(x => x.Content).CustomType("BinaryBlob");

            References(x => x.Experience).Not.Nullable();
        }
    }
}