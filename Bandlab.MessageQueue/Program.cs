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
        /// This program will connect with a service bus and create a collection in the database.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var connectionString = "Endpoint=sb://netservicequeue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=lcCq1MYmsRTqzyxvLbjBBGcOAWZrXVyakhvy2UmcpGE=";
            var queueName = "queuetest2";
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            //var fileData = System.IO.File.ReadAllBytes(@"C:\Users\AP7392\Pictures\data.png");
            //Console.WriteLine("File read");
            Console.WriteLine("Enter an album name: ");
            var albumname = Console.ReadLine();
            var message = new BrokeredMessage(albumname);

            Console.WriteLine(String.Format("Message id: {0}", message.MessageId));

            client.Send(message);

            Console.WriteLine("Message successfully sent! Press ENTER to exit program");
            Console.ReadLine();
            
        }
    }
}