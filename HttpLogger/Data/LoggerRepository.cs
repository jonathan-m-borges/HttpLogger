using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using HttpLogger.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;

namespace HttpLogger.Data
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly DocumentClient client;
        private readonly string databaseId;
        private readonly string collectionId;
        private readonly Uri collectionUri;

        public LoggerRepository(IConfiguration configuration)
        {
            var serviceEndpoint = configuration.GetValue<string>("documentdb:serviceEndpoint");
            var authKey = configuration.GetValue<string>("documentdb:authKey");
            databaseId = configuration.GetValue<string>("documentdb:databaseId");
            collectionId = configuration.GetValue<string>("documentdb:collectionId");
            collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
            
            client = new DocumentClient(new Uri(serviceEndpoint), authKey);
        }

        public async Task<IEnumerable<Logger>> GetAllAsync()
        {
            var query = client
                .CreateDocumentQuery<Logger>(collectionUri)
                .AsDocumentQuery();

            var results = new List<Logger>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<Logger>());
            }

            return results;
        }

        public async Task CreateAsync(Logger logger)
        {
            await client.CreateDocumentAsync(collectionUri, logger);
        }

        public async Task<Logger> GetByNameAsync(string name)
        {
            var response = await client.ReadDocumentAsync<Logger>(UriFactory.CreateDocumentUri(databaseId, collectionId, name));
            return response.Document;
        }

        public async Task UpdateAsync(Logger logger)
        {
            await client.UpsertDocumentAsync(collectionUri, logger);
        }

        public async Task RemoveAsync(string id){
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, id));
        }
    }
}