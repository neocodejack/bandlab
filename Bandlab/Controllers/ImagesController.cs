using Bandlab.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Bandlab.Controllers
{
    [RoutePrefix("api/v1")]
    public class ImagesController : ApiController
    {
        private string _baseUploadUrl;
        private string _baseDataUrl;

        public ImagesController()
        {
            _baseUploadUrl = ConfigurationManager.AppSettings["UploaderApi"].ToString();
            _baseDataUrl = ConfigurationManager.AppSettings["DataProcessorApi"].ToString();
        }

        [Route("gateway/uploadfile/{collectionId}")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFile(string collectionId)
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string filePath = string.Empty;
                string fileName = string.Empty;
                string contentType = string.Empty;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        fileName = postedFile.FileName;
                        contentType = postedFile.ContentType;
                        filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);
                    }
                }
 
                using (var client = new HttpClient())
                {
                    var fileStreamContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                    fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };
                    fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(fileStreamContent);
                        var uploadResponse = await client.PostAsync(_baseUploadUrl+"api/v1/blobs/upload", formData);
                        
                        if (uploadResponse.IsSuccessStatusCode)
                        {
                            //Deleting the local disk file
                            File.Delete(filePath);
                            var resultSet = uploadResponse.Content.ReadAsStringAsync().Result;
                            var fileDetails = JsonConvert.DeserializeObject<List<UploadResponseModel>>(resultSet);
                            //Code to Update database records post upload
                            var requestData = new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("CollectionId", collectionId),
                                new KeyValuePair<string, string>("FileName", fileDetails[0].FileName),
                                new KeyValuePair<string, string>("FileUrl", fileDetails[0].FileUrl),
                                new KeyValuePair<string, string>("FileSizeInBytes", fileDetails[0].FileSizeInBytes),
                                new KeyValuePair<string, string>("FileSizeInKb", fileDetails[0].FileSizeInKb)
                            };

                            HttpContent content = new FormUrlEncodedContent(requestData);
                            var saveResponse = await client.PostAsync(_baseDataUrl+"api/v1/collection/upload", content);
                            var responseDatainString = saveResponse.Content.ReadAsStringAsync().Result;
                            var response = JsonConvert.DeserializeObject<ResponseModel>(responseDatainString);
                            return Ok(response);
                        }
                        else
                        {
                            return InternalServerError();
                        }
                    }   
                }
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
        [Route("gateway/downloadfile/{fileId}")]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadFile(string fileId)
        {
            using(var client = new HttpClient())
            {
                var fileName = string.Empty;
                var fileresponse = client.GetAsync(_baseDataUrl + "api/v1/collection/images/" + fileId).Result;
                fileName = fileresponse.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = fileresponse.Content.ReadAsStringAsync().Result;
                    // Code to call the Download file service
                    var content = new StringContent(fileName);

                    var response = await client.PostAsync(_baseUploadUrl + "api/v1/blobs/download", content);  //The hardcoded url can be moved to config file
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
                var response = await client.GetAsync(_baseDataUrl + "api/v1/collection/images/" + fileId);
                var deletefileResponse = await client.GetAsync(_baseDataUrl + "api/v1/collection/delete/" + fileId);

                fileName = response.Content.ReadAsStringAsync().Result;
                
                if (!string.IsNullOrEmpty(fileName))
                {
                    var deletefileContent = new StringContent(fileName);
                    var deleteresponse = await client.PostAsync(_baseUploadUrl + "api/v1/blobs/delete", deletefileContent);  //The hardcoded url can be moved to config file 
                    if (deleteresponse.IsSuccessStatusCode)
                    {
                        return deleteresponse;
                    }
                    else
                    {
                        //Code to call the queue to delete the file
                        var connectionString = ConfigurationManager.AppSettings["ServiceBusConnectiongString"].ToString();
                        var queueName = ConfigurationManager.AppSettings["QueueName"].ToString();

                        ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;

                        var queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName);
                        var message = new BrokeredMessage(fileName);
                        //Sending th message to queue
                        queueClient.Send(message);

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
