using System;

namespace Badges.Models.Services
{
    public class BlobResult
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }

    public class BlobIdentity
    {
        public Guid Id { get; set; }
        public Uri Uri { get; set; }
    }
}