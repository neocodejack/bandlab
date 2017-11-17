using Bandlab.DataHandler;
using Bandlab.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bandlab.Provider
{
    public class StorageUploadProvider : MultipartFileStreamProvider
    {
        public List<UploadModel> Uploads { get; set; }
        private ObjectId _collectionId { get; set; }

        public StorageUploadProvider(string collectionId) : base(Path.GetTempPath())
        {
            _collectionId = ObjectId.Parse(collectionId);
            Uploads = new List<UploadModel>();
        }

        public override Task ExecutePostProcessingAsync()
        {
            var mongoHelper = new MongoDbHelper();

            foreach (var fileData in FileData)
            {
                // Sometimes the filename has a leading and trailing double-quote character
                // when uploaded, so we trim it; otherwise, we get an illegal character exception
                var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));

                // Retrieve reference to a blob
                var blobContainer = Helper.GetBlobContainer();
                var blob = blobContainer.GetBlockBlobReference(fileName);
                
                // Set the blob content type
                blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;

                // Upload file into blob storage, basically copying it from local disk into Azure
                using (var fs = File.OpenRead(fileData.LocalFileName))
                {
                    blob.UploadFromStream(fs);
                }

                // Delete local file from disk
                File.Delete(fileData.LocalFileName);
                
                //Adding to mongo collection
                var fileId = mongoHelper.Add(new Images { Name = blob.Name, CreatedDate = DateTime.Now, Collection = new List<ObjectId>{ _collectionId }, Metadata = new UploadModel { FileName = blob.Name, FileUrl = blob.Uri.AbsoluteUri, FileSizeInBytes = blob.Properties.Length } });

                // Create blob upload model with properties from blob info
                var blobUpload = new UploadModel
                {
                    FileId = Convert.ToString(fileId),
                    FileName = blob.Name,
                    FileUrl = blob.Uri.AbsoluteUri,
                    FileSizeInBytes = blob.Properties.Length
                };
                
                // Add uploaded blob to the list
                Uploads.Add(blobUpload);
            }

            return base.ExecutePostProcessingAsync();
        }
    }
}