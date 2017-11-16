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

namespace Bandlab.Controllers
{
    public class ImagesController : ApiController
    {
        // Interface in place so you can resolve with IoC container of your choice
        private readonly ICdnService _service;

        //Dependency Resolver using Unity
        public ImagesController(ICdnService service)
        {
            _service = service;
        }

        /// <summary>
        /// Uploads one or more blob files.
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/blobs/{collectionid}/upload")]
        [HttpPost]
        [ResponseType(typeof(List<UploadModel>))]
        public async Task<IHttpActionResult> PostBlobUpload(string collectionId)
        {
            try
            {
                // This endpoint only supports multipart form data
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                // Call service to perform upload, then check result to return as content
                var result = await _service.UploadFile(Request.Content,collectionId);
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
        [Route("api/v1/blobs/{blobId}/download")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetBlobDownload(string blobId)
        {
            try
            {
                var result = await _service.DownloadFile(blobId);
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
        /// Delete Blob Id
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns></returns>
        [Route("api/v1/blobs/{blobId}/delete")]
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteBlob(string blobId)
        {
            try
            {
                var result = await _service.DeleteFile(blobId);
                if (result == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(result+" is Deleted")
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/blob/images")]
        [HttpGet]
        public async Task<IHttpActionResult> GetImages()
        {
            try
            {
                var result = await _service.GetImages();
                return Ok(result);

            }catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Add a collection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/v1/collection/{name}")]
        [HttpPut]
        public async Task<IHttpActionResult> AddCollection(string name)
        {
            try
            {
                var result = await _service.AddCollection(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get list of collections
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/collections")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCollection()
        {
            try
            {
                var result = await _service.GetCollection();
                return Ok(result);

            }catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get Images By Collection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/v1/collection/{name}/images")]
        [HttpGet]
        public async Task<IHttpActionResult> GetImagesByCollection(string name)
        {
            try
            {
                var result = _service.GetImagesByCollection(name);
                return Ok(result);

            }catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Map Image to a collection
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [Route("api/v1/collection/map/{collectionId}/{imageId}")]
        [HttpPut]
        public async Task<IHttpActionResult> MapImageToCollection(string collectionId, string imageId)
        {
            try
            {
                var result = await _service.Map(collectionId, imageId);
                return Ok(result);

            }catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }     
    }
}
