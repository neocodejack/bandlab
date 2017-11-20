using Bandlab.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bandlab.DataProcessor.Controllers
{
    [RoutePrefix("api/v1")]
    public class DataController : ApiController
    {
        // Interface in place so you can resolve with IoC container of your choice
        private readonly IDataService _service;

        //Dependency Resolver using Unity
        public DataController(IDataService service)
        {
            _service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Route("collection/delete/{fileId}")]
        [HttpGet]
        public async Task<IHttpActionResult> DeteleRecord(string fileId)
        {
            try
            {
                var result = await _service.DeleteFile(fileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Route("collection/images/{fileId}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetFileNameById(string fileId)
        {
            try
            {
                var result = _service.GetBlobName(fileId);
                return new HttpResponseMessage
                {
                    Content = new StringContent(result, Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("collection/images")]
        [HttpGet]
        public async Task<IHttpActionResult> GetImages()
        {
            try
            {
                var result = await _service.GetImages();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Add a collection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("collection/{name}")]
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
        [Route("collection/")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCollection()
        {
            try
            {
                var result = await _service.GetCollection();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get Images By Collection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("collection/{name}/images")]
        [HttpGet]
        public async Task<IHttpActionResult> GetImagesByCollection(string name)
        {
            try
            {
                var result = _service.GetImagesByCollection(name);
                return Ok(result);

            }
            catch (Exception ex)
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
        [Route("collection/map/{collectionId}/{imageId}")]
        [HttpPut]
        public async Task<IHttpActionResult> MapImageToCollection(string collectionId, string imageId)
        {
            try
            {
                var result = await _service.Map(collectionId, imageId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
