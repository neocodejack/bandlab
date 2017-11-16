using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bandlab.Models
{
    public class Images
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ObjectId> Collection { get; set; }
        public BlobUploadModel Metadata { get; set; }
    }
}