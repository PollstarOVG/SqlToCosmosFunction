using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SqlToCosmosFunction.Models;

namespace SqlToCosmosFunction.Services
{
    public class CosmosService
    {
        private readonly Container _container;

        public CosmosService(IConfiguration _configuration)
        {
            var client = new CosmosClient(
                _configuration["CosmosDB:Endpoint"],
                _configuration["CosmosDB:Key"]
            );

            _container = client.GetContainer(
                _configuration["CosmosDB:DatabaseName"],
                _configuration["CosmosDB:ContainerName"]
            );
        }

        public async Task UpsertBatchAsync(IEnumerable<Artist> items)
        {
            var tasks = items.Select(item => 
                _container.UpsertItemAsync(item, new PartitionKey(item.Id.ToString()))
            );

            await Task.WhenAll(tasks);
        }
    }
}
