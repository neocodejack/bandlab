using System;
using System.Threading.Tasks;
using System.Net.Http;

public static void Run(string myQueueItem, TraceWriter log)
{
    log.Info(myQueueItem);
    //Deleting the file from blob
    using(var client = new HttpClient()){
        var content = new StringContent(myQueueItem);
        var response = client.PostAsync("http://websitertqfxdfcoswsa.azurewebsites.net/api/v1/blobs/delete",content).Result;
        log.Info(response.Content.ReadAsStringAsync().Result);
    }

}
