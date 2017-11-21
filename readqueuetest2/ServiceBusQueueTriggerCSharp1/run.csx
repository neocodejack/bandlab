using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

public static void Run(string myQueueItem, TraceWriter log)
{
    using (var httpClient = new HttpClient())
    {
        //Creating Collection
        var result =  httpClient.PutAsync("http://websitebvlervtqmyvk6.azurewebsites.net/api/v1/collection/"+myQueueItem,null).Result;
        string resultContent =  result.Content.ReadAsStringAsync().Result;
        log.Info(resultContent);
        log.Info($"Data Successfully Processed: {myQueueItem}");
    }
}
