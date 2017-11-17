using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;

namespace qsend
{
    class Program
    {
        /// <summary>
        /// This program will connect with a service bus and push an image to the queue
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var connectionString = "Endpoint=sb://netservicequeue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=lcCq1MYmsRTqzyxvLbjBBGcOAWZrXVyakhvy2UmcpGE=";
            var queueName = "queuetest2";
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var fileData = System.IO.File.ReadAllBytes(@"C:\Users\AP7392\Pictures\data.png");
            Console.WriteLine("File read");
            var message = new BrokeredMessage(fileData);

            Console.WriteLine(String.Format("Message id: {0}", message.MessageId));

            client.Send(message);

            Console.WriteLine("Message successfully sent! Press ENTER to exit program");
            Console.ReadLine();

            //var connectionString = "Endpoint=sb://netservicequeue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=lcCq1MYmsRTqzyxvLbjBBGcOAWZrXVyakhvy2UmcpGE=";
            //var queueName = "queuetest2";
            //ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;
            //var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            //client.OnMessage(message =>
            //{
            //    Console.WriteLine(message.GetBody<string>());
            //});

            //Console.ReadLine();
        }
    }
}