using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SqlToCosmosFunction.Interfaces;

namespace SqlToCosmosFunction
{
    public class SyncFunction
    {
        private readonly ILogger<SyncFunction> _logger;
        private readonly ISqlService _sqlService;
        private readonly ICosmosService _cosmosService;

        public SyncFunction(ILogger<SyncFunction> logger, ISqlService sqlService, ICosmosService cosmosService)
        {
            _logger = logger;
            _sqlService = sqlService;
            _cosmosService = cosmosService;
        }

        [Function("SyncFunction")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            try
            {
                if (req == null)
                {
                    _logger.LogWarning("Invalid or missing 'batchSize' query parameter.");

                    var data = await _sqlService.GetDataAsync(1);
                    if (data == null || !data.Any())
                    {
                        _logger.LogInformation("No data found to sync.");
                    }
                    else
                    {
                        await _cosmosService.UpsertBatchAsync(data);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
            }
        }
    }
}
