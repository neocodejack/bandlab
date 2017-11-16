using Bandlab.Helpers;
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
        private MongoDbHelper mongoDbHelper;
        public CdnService()
        {
            mongoDbHelper = new MongoDbHelper();
        }
        public async Task<List<UploadModel>> UploadFile(HttpContent httpContent, string collectionId)
        {
            var uploadProvider = new StorageUploadProvider(collectionId);
            
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

        public async Task<DownloadModel> DownloadFile(string blobId)
        {
            var blobName = GetBlobName(blobId);
            
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

        public async Task<string> DeleteFile(string blobId)
        {
            var blobName = GetBlobName(blobId);

            if (!string.IsNullOrEmpty(blobName))
            {
                var container = Helper.GetBlobContainer();
                var blob = container.GetBlockBlobReference(blobName);

                await blob.DeleteAsync();
                await mongoDbHelper.Delete(blobId);
                return blobName;
            }

            return null;
        }

        public async Task<Collection> AddCollection(string name)
        {
            var mongoDbHelper = new MongoDbHelper();
            var objectId = await mongoDbHelper.AddCollection(name);
            return new Collection { Id = objectId, Name = name };
        }

        public async Task<List<Collection>> GetCollection()
        {
            var mongoDbHelper = new MongoDbHelper();
            return mongoDbHelper.GetCollections();
        }

        public List<Images> GetImagesByCollection(string name)
        {
            var mongoDbHelper = new MongoDbHelper();
            return mongoDbHelper.GetImagesByCollection(name);
        }
        
        private string GetBlobName(string blobId)
        {
            var mongoDbHelper = new MongoDbHelper();
            return mongoDbHelper.Find(blobId);
        }

        public async Task<Images> Map(string collectionId, string imageId)
        {
            var mongoDbHelper = new MongoDbHelper();
            return await mongoDbHelper.Map(collectionId, imageId);
        }

        public Task<List<Images>> GetImages()
        {
            var mongoDbHelper = new MongoDbHelper();
            return mongoDbHelper.GetImages().Result;
        }
    }
}