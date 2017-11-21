#r "Newtonsoft.Json" 
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;


public static async Task<object> Run(HttpRequestMessage myQueueItem, TraceWriter log)
{
    //uploadedMessage = JsonConvert.DeserializeObject<QueueMessageData>(myQueueItem.GetBody<string>());
    using (var client = new HttpClient())
    {
        //log.Info(myQueueItem);
        
        // var fileStreamContent = new ByteArrayContent(myQueueItem.fileData);
        // fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = myQueueItem.fileName };


        // fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(myQueueItem.contentType);
        // using (var formData = new MultipartFormDataContent())
        // {
        //     formData.Add(fileStreamContent);
        //     var uploadResponse = client.PostAsync("http://websitertqfxdfcoswsa.azurewebsites.net/api/v1/blobs/upload", formData);

        //     // //Code to Update database records post upload
        //     var requestData = new List<KeyValuePair<string, string>>
        //     {
        //          new KeyValuePair<string, string>("CollectionId", "5a0d2882e786975004c31f05"),
        //          new KeyValuePair<string, string>("FileName", myQueueItem.fileName),
        //          new KeyValuePair<string, string>("FileUrl", "0"),
        //          new KeyValuePair<string, string>("FileSizeInBytes", "0"),
        //          new KeyValuePair<string, string>("FileSizeInKb", "0")
        //     };

        //     HttpContent content = new FormUrlEncodedContent(requestData);
        //     var saveResponse = client.PostAsync("http://websitebvlervtqmyvk6.azurewebsites.net/api/v1/collection/upload", content);
            
            
        //     log.Info("Success");
            
        //}   
    }
    log.Info($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
}
