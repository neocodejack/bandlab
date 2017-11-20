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
                var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));
                
                var blobContainer = Helper.GetBlobContainer();
                var blob = blobContainer.GetBlockBlobReference(fileName);
                
                blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;
                
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