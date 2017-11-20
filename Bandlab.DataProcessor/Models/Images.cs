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
        public ImageMetaData Metadata { get; set; }
    }

    public class ImageMetaData
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long FileSizeInBytes { get; set; }
        public long FileSizeInKb { get { return (long)Math.Ceiling((double)FileSizeInBytes / 1024); } }
    }

    public class EndpointApiRequestData : ImageMetaData
    {
        public string CollectionId { get; set; }

    }
}