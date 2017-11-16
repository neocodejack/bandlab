using Bandlab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bandlab.Services
{
    public interface IBlobService
    {
        Task<List<BlobUploadModel>> UploadBlobs(HttpContent httpContent, string collectionId);
        Task<BlobDownloadModel> DownloadBlob(string blobId);
        Task<string> DeleteBlob(string blobId);
        Task<Collection> AddCollection(string name);
        Task<List<Collection>> GetCollection();
        List<Images> GetImagesByCollection(string name);
        Task<Images> Map(string collectionId, string imageId);
        Task<List<Images>> GetImages();
    }
}