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
}