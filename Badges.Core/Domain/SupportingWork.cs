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

        public virtual string Notes { get; set; }
        public virtual string Url { get; set; }

        [Required] //Type of work, text, photo, video, etc
        public virtual string Type { get; set; }

        [Required]
        public virtual Experience Experience { get; set; }

        public virtual string GetEmbedUrl()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                return string.Empty;
            }

            if (Url.Contains("youtu.be") || Url.Contains("youtube.com"))
            {
                //Url is either /watch?v=[code] or /[code] 
                return "//www.youtube.com/embed/" +
                       Url.Substring(Url.Contains("watch?")
                                         ? Url.LastIndexOf("v=", System.StringComparison.Ordinal) + 2
                                         : Url.LastIndexOf("/", System.StringComparison.Ordinal) + 1
                           );
            }

            if (Url.Contains("vimeo.com"))
            {
                return "//player.vimeo.com/video/" +
                       Url.Substring(Url.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
            }

            return string.Empty; //TODO: include vimeo
        }
    }

    public static class SupportingWorkTypes
    {
        public const string Text = "text"; 
        public const string Photo = "photo";
        public const string Video = "video";
    }

    public class SupportingWorkMap : ClassMap<SupportingWork>
    {
        public SupportingWorkMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Description).Not.Nullable();
            
            Map(x => x.Name);
            Map(x => x.ContentType);
            Map(x => x.Content).CustomType("BinaryBlob");

            Map(x => x.Url);
            Map(x => x.Notes);

            Map(x => x.Type);

            References(x => x.Experience).Not.Nullable();
        }
    }
}