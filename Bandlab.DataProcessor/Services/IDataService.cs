using Bandlab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bandlab.Services
{
    public interface IDataService
    {
        Images UploadFile(Images entity);
        Task<string> DeleteFile(string blobId);
        Task<Collection> AddCollection(string name);
        Task<List<Collection>> GetCollection();
        List<Images> GetImagesByCollection(string name);
        Task<Images> Map(string collectionId, string imageId);
        Task<List<Images>> GetImages();
        string GetBlobName(string blobId);
    }
}