using Bandlab.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bandlab.DataHandler
{
    public class MongoDbHelper
    {
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<Images> _imageCollection;

        //Constructor initialization for Mongo Connection
        public MongoDbHelper()
        {
            _client = new MongoClient("mongodb://myimages:Seeeg5e1zYaooE9kRrg18SwKZiEkvmQpz0cSxUOdFo5U3Gh0pvzxz3UTTCogIXJ9JJ92hJKwCBcZCBVqa5QEDQ==@myimages.documents.azure.com:10255/?ssl=true&replicaSet=globaldb");
            _database = _client.GetDatabase("imagecollection");
            _imageCollection = _database.GetCollection<Images>("images");
        }

        //Method to Add records to collection
        public ObjectId Add(Images entity)
        {
            _imageCollection.InsertOne(entity);
            return _imageCollection.AsQueryable().Where(l => l.Name.Equals(entity.Name)).AsQueryable().OrderByDescending(o => o.CreatedDate).AsQueryable().Select(x => x.Id).FirstOrDefault();
        }

        public string Find(string id)
        {
            var name = _imageCollection.AsQueryable().Where(l => l.Id == ObjectId.Parse(id)).AsQueryable().Select(x => x.Name).FirstOrDefault();
            return name;
        }

        public async Task<DeleteResult> Delete(string id)
        {
            var filterBuilder = Builders<Images>.Filter;
            var filter = filterBuilder.Eq(x => x.Id, ObjectId.Parse(id));
            return await _imageCollection.DeleteOneAsync(filter);
        }

        public async Task<ObjectId> AddCollection(string name)
        {
            var _collection = _database.GetCollection<Collection>("collection");
            var _existingCollection = GetCollections();
            if (_existingCollection.Find(x => x.Name.Equals(name)) == null)
            {
                var entity = new Collection { Name = name };
                await _collection.InsertOneAsync(entity);
                return _collection.AsQueryable().Where(l => l.Name.Equals(name)).AsQueryable().Select(x => x.Id).FirstOrDefault();
            }

            return ObjectId.Empty;
        }

        public List<Collection> GetCollections()
        {
            var _collection = _database.GetCollection<Collection>("collection");
            var filterBuilder = Builders<Collection>.Filter.Empty;
            return _collection.Find(filterBuilder).ToList();
        }

        public List<Images> GetImagesByCollection(string name)
        {
            var collectionId = GetCollections().Where(colName => colName.Name.Equals(name)).Select(colId => colId.Id).FirstOrDefault();
            return _imageCollection.AsQueryable().Where(l => l.Collection.Contains(collectionId)).ToList();
        }

        public async Task<Images> Map(string collectionId, string imageId)
        {
            var existingCollectionIds = _imageCollection.AsQueryable().Where(l => l.Id == ObjectId.Parse(imageId)).Select(colIds => colIds.Collection).FirstOrDefault();
            if (!existingCollectionIds.Contains(ObjectId.Parse(collectionId)))
            {
                existingCollectionIds.Add(ObjectId.Parse(collectionId));
                FilterDefinition<Images> filter = null;
                UpdateDefinition<Images> update = null;
                var filterBuilder = Builders<Images>.Filter;
                filter = filterBuilder.Eq("Id", ObjectId.Parse(imageId));
                update = Builders<Images>.Update.Set("Collection", existingCollectionIds);
                await _imageCollection.UpdateOneAsync(filter, update);
            }

            return null;
        }

        public async Task<Task<List<Images>>> GetImages() => _imageCollection.Find(Builders<Images>.Filter.Empty).ToListAsync();
    }
    
   
}