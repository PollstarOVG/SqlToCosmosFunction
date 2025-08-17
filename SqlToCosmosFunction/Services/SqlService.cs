using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SqlToCosmosFunction.Interfaces;
using SqlToCosmosFunction.Models;
using static SqlToCosmosFunction.Options.DatabaseOptions;

namespace SqlToCosmosFunction.Services
{
    public class SqlService : ISqlService
    {
        private readonly string _connectionString;

        public SqlService(IOptions<SqlDbSettings> options)
        {
            _connectionString = options.Value.ConnectionString
                ?? throw new ArgumentNullException(nameof(options.Value.ConnectionString));
        }

        public async Task<IEnumerable<Artist>> GetDataAsync(int batchSize)
        {
            var results = new List<Artist>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = GetQuery();
            var cmd = new SqlCommand(query, conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read() && reader.HasRows)
            {
                var artist = new Artist
                {
                    Id = Guid.NewGuid().ToString(),
                    ArtistId = Convert.ToString(reader.GetInt32(0)),
                    Name = reader.GetString(1),
                    SortName = reader.GetString(2),
                    SearchName = reader.GetString(3),
                    ArtistType = reader.GetString(4),
                    Type = reader.GetString(4),
                    Genre = [reader.GetString(5)]
                };

                results.Add(artist);
            }

            return results;
        }

        private static string GetQuery()
        {
            return @"
                    SELECT TOP 2 a.Id, a.ArtistName, a.SortName, a.SearchName, at.Description, g.GenreName
                        FROM Artist as a
                    INNER JOIN ArtistGenre as ag on a.Id = ag.ArtistId
                    INNER JOIN ArtistType at on a.ArtistTypeId = at.Id
                    INNER JOIN Genre as g on ag.GenreId = G.Id
                    ORDER BY a.Id
                ";
        }
    }
}