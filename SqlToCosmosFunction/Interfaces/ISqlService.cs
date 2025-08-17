using SqlToCosmosFunction.Models;

namespace SqlToCosmosFunction.Interfaces
{
    public interface ISqlService
    {
        Task<IEnumerable<Artist>> GetDataAsync(int batchSize);
    }
}
