using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    /// <summary>
    /// Supporting work for an experience
    /// Can be photo or file (in the future, audio & video as well)
    /// </summary>
    public class SupportingFile : DomainObjectGuid
    {
        [Required]
        public virtual string Description { get; set; }

        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string ContentType { get; set; }

        [Required]
        public virtual byte[] Content { get; set; }

        [Required]
        public virtual Experience Experience { get; set; }
    }

    public class SupportingFileMap : ClassMap<SupportingFile>
    {
        public SupportingFileMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name).Not.Nullable();
            Map(x => x.ContentType).Not.Nullable();
            Map(x => x.Description).Not.Nullable();
            Map(x => x.Content).CustomType("BinaryBlob");

            References(x => x.Experience).Not.Nullable();
        }
    }
}