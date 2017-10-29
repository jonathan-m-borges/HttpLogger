using System;
using System.IO;
using System.Threading.Tasks;
using HttpLoggerModel;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace HttpLoggerJob
{
    class Program
    {
        private static IConfigurationRoot configuration;
        private static CloudQueue queue;

        static void Main(string[] args)
        {
            BuildConfiguration();
            
            StorageAccountInit();

            Run().Wait();
        }

        private static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");

            configuration = builder.Build();
        }

        private static void StorageAccountInit()
        {
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(configuration["storage:connectionString"]);
            var queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference(configuration["storage:queueReference"]);
            queue.CreateIfNotExistsAsync();
        }

        private static async Task Run()
        {
            while (true)
            {
                var message = await queue.GetMessageAsync();
                //JsonConvert.DeserializeObject(message.AsString);
                if (message == null)
                {
                    await Task.Delay(5000);
                    Console.WriteLine("Queue empty...");
                    continue;
                }

                ProcessMessage(message);

                Console.WriteLine("Message recebida...");
                Console.WriteLine(message.AsString);
            }
        }

        private static void ProcessMessage(CloudQueueMessage message)
        {
            Console.WriteLine("Message recebida...");
            Console.WriteLine(message.AsString);

            try {
                var request = JsonConvert.DeserializeObject<Request>(message.AsString);
            }
            catch (Exception e)
            {

            }
            


        }
    }
}
