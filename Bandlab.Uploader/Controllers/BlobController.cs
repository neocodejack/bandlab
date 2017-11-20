using Bandlab.Models;
using Bandlab.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Bandlab.Uploader.Controllers
{
    public class BlobController : ApiController
    {
        // Interface in place so you can resolve with IoC container of your choice
        private readonly ICdnService _service;

        //Dependency Resolver using Unity
        public BlobController(ICdnService service)
        {
            _service = service;
        }

        /// <summary>
        /// Uploads one or more blob files.
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/blobs/upload")]
        [HttpPost]
        [ResponseType(typeof(List<UploadModel>))]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                // This endpoint only supports multipart form data
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                // Call service to perform upload, then check result to return as content
                var result = await _service.UploadFile(Request.Content);
                if (result != null && result.Count > 0)
                {
                    return Ok(result);
                }

                // Otherwise
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Downloads a blob file.
        /// </summary>
        /// <param name="blobId">The ID of the blob.</param>
        /// <returns></returns>
        [Route("api/v1/blobs/download")]
        [HttpPost]
        public async Task<HttpResponseMessage> Download()
        {
            try
            {

                var fileName = Request.Content.ReadAsStringAsync().Result;
                var result = await _service.DownloadFile(fileName);
                if (result == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                // Reset the stream position; otherwise, download will not work
                result.BlobStream.Position = 0;

                // Create response message with blob stream as its content
                var message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(result.BlobStream)
                };

                message.Content.Headers.ContentLength = result.BlobLength;
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(result.BlobContentType);
                message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = HttpUtility.UrlDecode(result.BlobFileName),
                    Size = result.BlobLength
                };

                return message;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Delete Blob
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns></returns>
        [Route("api/v1/blobs/delete")]
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteBlob()
        {
            try
            {
                var fileName = Request.Content.ReadAsStringAsync().Result;
                var result = await _service.DeleteFile(fileName);
                if (result == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(result + " is Deleted")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}
