using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bandlab.Controllers
{
    [RoutePrefix("api/v1")]
    public class ImagesController : ApiController
    {
        [Route("gateway/uploadfile/{collectionId}")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFile(string collectionId)
        {
            using(var client = new HttpClient())
            {
                HttpContent fileStreamContent = new StreamContent(Request.Content.ReadAsStreamAsync().Result);
                using(var formData = new MultipartFormDataContent())
                {
                    formData.Add(fileStreamContent);
                    var response = await client.PostAsync("http://localhost:49721/api/v1/blobs/upload", formData);
                    return Ok(response);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Route("gateway/downloadfile/{fileId}")]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadFile(string fileId)
        {
            using(var client = new HttpClient())
            {
                var fileName = string.Empty;
                var fileresponse = client.GetAsync("http://localhost:49730/api/v1/collection/images/" + fileId).Result;
                fileName = fileresponse.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = fileresponse.Content.ReadAsStringAsync().Result;
                    // Code to call the Download file service
                    var content = new StringContent(fileName);

                    var response = await client.PostAsync("http://localhost:49721/api/v1/blobs/download", content);  //The hardcoded url can be moved to config file 
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                    else
                    {
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("Unable to process request Now"),
                            StatusCode = HttpStatusCode.InternalServerError
                        };
                    }
                }
                else
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("Unable to process request Now"),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }
                
            }
        }

        /// <summary>
        /// Orchastration to delete a file from both blob and db
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Route("gateway/deletefile/{fileId}")]
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteFile(string fileId)
        {
            using(var client = new HttpClient())
            {
                var fileName = string.Empty;
                var response = await client.GetAsync("http://localhost:49730/api/v1/collection/images/" + fileId);
                var deletefileResponse = await client.GetAsync("http://localhost:49730/api/v1/collection/delete/" + fileId);

                fileName = response.Content.ReadAsStringAsync().Result;
                
                if (!string.IsNullOrEmpty(fileName))
                {
                    var deletefileContent = new StringContent(fileName);
                    var deleteresponse = await client.PostAsync("http://localhost:49721/api/v1/blobs/delete", deletefileContent);  //The hardcoded url can be moved to config file 
                    if (deleteresponse.IsSuccessStatusCode)
                    {
                        return deleteresponse;
                    }
                    else
                    {
                        //Code to call the queue to delete the file
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("Unable to process request Now, the request has been moved to the queue"),
                            StatusCode = HttpStatusCode.InternalServerError
                        };
                    }
                }
                else
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("fileName is blank"),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }
            }
        }
            
    }
}
