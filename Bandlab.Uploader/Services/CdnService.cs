using Bandlab.DataHandler;
using Bandlab.Models;
using Bandlab.Provider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bandlab.Services
{
    public class CdnService : ICdnService
    {
        public async Task<List<UploadModel>> UploadFile(HttpContent httpContent)
        {
            var uploadProvider = new StorageUploadProvider();
            
            var list = await httpContent.ReadAsMultipartAsync(uploadProvider)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        throw task.Exception;
                    }

                    var provider = task.Result;
                    return provider.Uploads.ToList();
                });

            
            return list;
        }

        public async Task<DownloadModel> DownloadFile(string blobName)
        {
            if (!String.IsNullOrEmpty(blobName))
            {
                var container = Helper.GetBlobContainer();
                var blob = container.GetBlockBlobReference(blobName);

                var ms = new MemoryStream();
                await blob.DownloadToStreamAsync(ms);
                
                var lastPos = blob.Name.LastIndexOf('/');
                var fileName = blob.Name.Substring(lastPos + 1, blob.Name.Length - lastPos - 1);
                
                var download = new DownloadModel
                {
                    BlobStream = ms,
                    BlobFileName = fileName,
                    BlobLength = blob.Properties.Length,
                    BlobContentType = blob.Properties.ContentType
                };

                return download;
            }
            
            return null;
        }

        public async Task<string> DeleteFile(string blobName)
        {
            if (!string.IsNullOrEmpty(blobName))
            {
                var container = Helper.GetBlobContainer();
                var blob = container.GetBlockBlobReference(blobName);

                await blob.DeleteAsync();
                return blobName;
            }

            return null;
        }
        
    }
}