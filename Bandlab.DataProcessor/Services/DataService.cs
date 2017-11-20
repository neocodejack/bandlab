using Bandlab.DataHandler;
using Bandlab.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bandlab.Services
{
    public class DataService : IDataService
    {
        private MongoDbHelper mongoDbHelper;
        public DataService()
        {
            mongoDbHelper = new MongoDbHelper();
        }
        public Images UploadFile(Images entity)
        {
            var objectId = mongoDbHelper.Add(entity);
            entity.Id = objectId;
            return entity;
        }
        
        public async Task<string> DeleteFile(string blobId)
        {
            var blobName = GetBlobName(blobId);

            if (!string.IsNullOrEmpty(blobName))
            {   
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
        
        public string GetBlobName(string blobId)
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