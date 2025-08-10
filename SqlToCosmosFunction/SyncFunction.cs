using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlToCosmosFunction.Services;

namespace SqlToCosmosFunction
{
    public class SyncFunction
    {
        private readonly ILogger<SyncFunction> _logger;
        private readonly SqlService _sqlService;    
        private readonly CosmosService _cosmosService;
        private readonly int _batchSize;

        public SyncFunction(ILogger<SyncFunction> logger, SqlService sqlService, CosmosService cosmosService, IConfiguration config)
        {
            _logger = logger;
            _sqlService = sqlService;   
            _cosmosService = cosmosService;
            _batchSize = int.Parse(config["BatchSize"] ?? "100");
        }

        [Function("SyncFunction")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            try
            {
                var data = await _sqlService.GetDataAsync(_batchSize);
                if (data == null || !data.Any())
                {
                    _logger.LogInformation("No data found to sync.");
                }

                if (data.Any())
                {
                    await _cosmosService.UpsertBatchAsync(data);
                    _logger.LogInformation($"Successfully synced {data.Count()} items to Cosmos DB.");
                }
                else
                {
                    _logger.LogInformation("No new data to sync.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
            }
        }
    }
}
