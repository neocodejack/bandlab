using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bandlab.Models
{
    public class UploadResponseModel
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileSizeInBytes { get; set; }
        public string FileSizeInKb { get; set; } 
    }

    public class ResponseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Collection { get; set; }
        public UploadResponseModel Metadata { get; set; }
    }
}