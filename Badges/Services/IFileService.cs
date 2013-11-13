using System;
using System.IO;
using System.Web;
using Badges.Models.Services;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Configuration;

namespace Badges.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Save a file and return the URL for that file
        /// </summary>
        /// <returns></returns>
        BlobIdentity Save(HttpPostedFileBase file, bool publicAccess = false);

        /// <summary>
        /// Save a file and return the URL for that file
        /// </summary>
        /// <returns></returns>
        BlobIdentity Save(Stream stream, string contentType, bool publicAccess = false);

        /// <summary>
        /// Save two versions of the same file, but return the modified version for consumption
        /// </summary>
        /// <returns></returns>
        BlobIdentity Save(Stream originalStream, Stream modifiedStream, string contentType, bool publicAccess = false);

        void Delete(Guid id, bool publicAccess = false);

        /// <summary>
        /// We'd like to have a new ID generated after updating, so just delete and save
        /// </summary>
        BlobIdentity Update(Guid imageId, HttpPostedFileBase file, bool publicAccess = false);

        BlobResult Get(Guid id, bool publicAccess = false);
    }

    public class FileService : IFileService
    {
        private const string OriginalIdentifier = "_original";
        private readonly CloudBlobContainer _container;
        private readonly CloudBlobContainer _publicContainer;

        public FileService()
        {
            var storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            _container = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("FileContainer"));
            _container.CreateIfNotExists();
            _container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off });

            _publicContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("PublicImageContainer"));
            _publicContainer.CreateIfNotExists();
            _publicContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
        }

        /// <summary>
        /// Save a file and return the URL for that file
        /// </summary>
        /// <returns></returns>
        public BlobIdentity Save(HttpPostedFileBase file, bool publicAccess = false)
        {
            var container = publicAccess ? _publicContainer : _container;

            var blobId = Guid.NewGuid();
            
            var blob = container.GetBlockBlobReference(blobId.ToString());
            blob.UploadFromStream(file.InputStream);
            SetContentType(blob, file.ContentType);
            
            return new BlobIdentity {Id = blobId, Uri = blob.Uri};
        }

        /// <summary>
        /// Save a file and return the URL for that file
        /// </summary>
        /// <returns></returns>
        public BlobIdentity Save(Stream stream, string contentType, bool publicAccess = false)
        {
            var container = publicAccess ? _publicContainer : _container;

            var blobId = Guid.NewGuid();
            
            var blob = container.GetBlockBlobReference(blobId.ToString());
            stream.Seek(0, SeekOrigin.Begin);
            blob.UploadFromStream(stream);
            SetContentType(blob, contentType);
            
            return new BlobIdentity {Id = blobId, Uri = blob.Uri};
        }

        /// <summary>
        /// Save two versions of the same file, but return the modified version for consumption
        /// </summary>
        /// <returns></returns>
        public BlobIdentity Save(Stream originalStream, Stream modifiedStream, string contentType, bool publicAccess = false)
        {
            var container = publicAccess ? _publicContainer : _container;

            var blobId = Guid.NewGuid();
            
            var blob = container.GetBlockBlobReference(blobId.ToString());
            modifiedStream.Seek(0, SeekOrigin.Begin);
            blob.UploadFromStream(modifiedStream);
            SetContentType(blob, contentType);

            var originalBlob = container.GetBlockBlobReference(blobId + OriginalIdentifier);
            originalStream.Seek(0, SeekOrigin.Begin);
            originalBlob.UploadFromStream(originalStream);
            SetContentType(originalBlob, contentType);

            return new BlobIdentity {Id = blobId, Uri = blob.Uri};
        }

        public void Delete(Guid id, bool publicAccess = false)
        {
            var container = publicAccess ? _publicContainer : _container;

            var blob = container.GetBlockBlobReference(id.ToString());
            blob.Delete();
        }

        /// <summary>
        /// We'd like to have a new ID generated after updating, so just delete and save
        /// </summary>
        public BlobIdentity Update(Guid imageId, HttpPostedFileBase file, bool publicAccess = false)
        {
            Delete(imageId, publicAccess);
            return Save(file, publicAccess);
        }

        public BlobResult Get(Guid id, bool publicAccess = false)
        {
            var container = publicAccess ? _publicContainer : _container;

            var blob = container.GetBlockBlobReference(id.ToString());
            
            var result = new BlobResult {ContentType = blob.Properties.ContentType};

            using (var stream = new MemoryStream())
            {
                blob.DownloadToStream(stream);
                using (var reader = new BinaryReader(stream))
                {
                    stream.Position = 0;
                    result.Content = reader.ReadBytes((int)stream.Length);
                }
            }

            return result;
        }

        private static void SetContentType(ICloudBlob blob, string contentType)
        {
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                blob.Properties.ContentType = contentType;
                blob.SetProperties();
            }
        }
    }
}