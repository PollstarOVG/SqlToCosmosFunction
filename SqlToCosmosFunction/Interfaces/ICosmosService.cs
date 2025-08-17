using SqlToCosmosFunction.Models;

namespace SqlToCosmosFunction.Interfaces
{
    public interface ICosmosService
    {
        Task UpsertBatchAsync(IEnumerable<Artist> items);
    }
}
