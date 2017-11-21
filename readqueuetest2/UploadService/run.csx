#r "Newtonsoft.Json" 
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

public static void Run(HttpRequestMessage myQueueItem, TraceWriter log)
{
    var obj = JsonConvert.DeserializeObject<UploadModel>(myQueueItem.Content.ReadAsStringAsync().Result);
    //log.Info($"C# ServiceBus queue trigger function processed message: {jsonContent}");
}

public class UploadModel{
    public byte[] fileData { get; set; }
    public string contentType { get; set; }
    public string fileName { get; set; }
}