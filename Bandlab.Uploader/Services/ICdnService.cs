using Bandlab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bandlab.Services
{
    public interface ICdnService
    {
        Task<List<UploadModel>> UploadFile(HttpContent httpContent);
        Task<DownloadModel> DownloadFile(string blobName);
        Task<string> DeleteFile(string blobName);
    }
}