using System;
using System.IO;
using System.Threading.Tasks;
using HttpLoggerModel;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace HttpLoggerJob
{
    class Program
    {
        private IConfigurationRoot configuration;
        private CloudQueue queue;
        private DocumentClient documentDbClient;
        private Uri collectionUri;

        static void Main(string[] args)
        {
            try
            {
                var program = new Program();
                program.Run().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, e);
            }
        }

        public async Task Run()
        {
            BuildConfiguration();
            
            await StorageAccountInit();

            await DocumentDbInit();

            await ListenMessages();
        }

        private async Task ListenMessages()
        {
            while (true)
            {
                var message = await queue.GetMessageAsync();
                if (message == null)
                {
                    await Task.Delay(5000);
                    Console.WriteLine("Queue empty...");
                    continue;
                }

                await ProcessMessage(message);

                Console.WriteLine("Message recebida...");
                Console.WriteLine(message.AsString);
            }
        }

        private void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");

            configuration = builder.Build();
        }

        private async Task StorageAccountInit()
        {
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(configuration["storage:connectionString"]);
            var queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference(configuration["storage:queueReference"]);
            await queue.CreateIfNotExistsAsync();
        }

        private async Task DocumentDbInit()
        {
            documentDbClient = new DocumentClient(new Uri(configuration["documentdb:serviceEndpoint"]), configuration["documentdb:authKey"]);
            await documentDbClient.CreateDatabaseIfNotExistsAsync(new Database{Id=configuration["documentdb:databaseId"]});
            await documentDbClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(configuration["documentdb:databaseId"]), new DocumentCollection {Id=configuration["documentdb:collectionId"]});
            collectionUri = UriFactory.CreateDocumentCollectionUri(configuration["documentdb:databaseId"], configuration["documentdb:collectionId"]);
        }

        private async Task ProcessMessage(CloudQueueMessage message)
        {
            Console.WriteLine("Message recebida...");
            Console.WriteLine(message.AsString);

            try {
                var request = JsonConvert.DeserializeObject<Request>(message.AsString);
                
                await SaveRequest(request);
                
                await queue.DeleteMessageAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao processar request {message.Id}: {e.Message}", e);
            }
        }

        private async Task SaveRequest (Request request)
        {
            await documentDbClient.CreateDocumentAsync(collectionUri, request);
        }
    }
}
