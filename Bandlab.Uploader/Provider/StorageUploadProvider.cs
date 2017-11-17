using Bandlab.DataHandler;
using Bandlab.Models;
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
        
        public StorageUploadProvider() : base(Path.GetTempPath())
        {
            Uploads = new List<UploadModel>();
        }

        public override Task ExecutePostProcessingAsync()
        {
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
                
                
                // Create blob upload model with properties from blob info
                var blobUpload = new UploadModel
                {
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