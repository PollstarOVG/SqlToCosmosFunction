using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlToCosmosFunction.Interfaces;
using SqlToCosmosFunction.Models;
using static SqlToCosmosFunction.Options.DatabaseOptions;

namespace SqlToCosmosFunction.Services
{
    public class CosmosService : ICosmosService
    {
        private readonly CosmosClient _client;
        private readonly CosmosDbSettings _settings;
        private readonly Container _container;
        private readonly ILogger<CosmosService> _logger;

        public CosmosService(IOptions<CosmosDbSettings> options, ILogger<CosmosService> logger)
        {
            _settings = options.Value;

            _client = new CosmosClient(_settings.Endpoint, _settings.AuthKey, new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway
            });

            var database = _client.GetDatabase(_settings.DatabaseName);
            _container = database.GetContainer(_settings.ContainerName);

            _logger = logger;
        }

        public async Task UpsertBatchAsync(IEnumerable<Artist> items)
        {
            var count = 0;

            foreach (var artist in items)
            {
                try
                {
                    var query = new QueryDefinition("SELECT * FROM c WHERE c.artistId = @artistId")
                        .WithParameter("@artistId", artist.ArtistId);

                    using var iterator = _container.GetItemQueryIterator<Artist>(query);

                    var exists = false;
                    while (iterator.HasMoreResults && !exists)
                    {
                        var response = await iterator.ReadNextAsync();
                        exists = response.Any(a => a.ArtistId == artist.ArtistId);
                    }

                    if (!exists)
                    {
                        await _container.CreateItemAsync(artist, new PartitionKey(artist.ArtistId));
                        count++;
                    }
                }
                catch (CosmosException ex)
                {
                    Console.WriteLine($"Error inserting item with Id {artist.Id}: {ex.Message}");
                }
            }

            _logger.LogInformation($"Successfully synced {count} new items to Cosmos DB.");
        }
    }
}
