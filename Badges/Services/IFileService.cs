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
        Guid Save(HttpPostedFileBase file);

        BlobResult Get(Guid id);

        /// <summary>
        /// We'd like to have a new ID generated after updating, so just delete and save
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="file"></param>
        Guid Update(Guid imageId, HttpPostedFileBase file);
    }

    public class FileService : IFileService
    {
        private readonly CloudBlobContainer _container;

        public FileService()
        {
            var storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            _container = blobClient.GetContainerReference("imagesdev");
            _container.CreateIfNotExists();
        }

        /// <summary>
        /// Save a file and return the URL for that file
        /// </summary>
        /// <returns></returns>
        public Guid Save(HttpPostedFileBase file)
        {
            var blobUri = Guid.NewGuid();
            
            var blob = _container.GetBlockBlobReference(blobUri.ToString());
            blob.UploadFromStream(file.InputStream);
            SetContentType(blob, file.ContentType);
            
            return blobUri;
        }

        public void Delete(Guid id)
        {
            var blob = _container.GetBlockBlobReference(id.ToString());
            blob.Delete();
        }

        /// <summary>
        /// We'd like to have a new ID generated after updating, so just delete and save
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="file"></param>
        public Guid Update(Guid imageId, HttpPostedFileBase file)
        {
            Delete(imageId);
            return Save(file);
        }

        public BlobResult Get(Guid id)
        {
            var blob = _container.GetBlockBlobReference(id.ToString());
            
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