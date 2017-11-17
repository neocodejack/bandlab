using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bandlab.Models
{
    public class UploadModel
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long FileSizeInBytes { get; set; }
        public long FileSizeInKb { get { return (long)Math.Ceiling((double)FileSizeInBytes / 1024); } }
    }
}