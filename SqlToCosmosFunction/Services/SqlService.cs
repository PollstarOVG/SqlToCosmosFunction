using Microsoft.Data.SqlClient;
using SqlToCosmosFunction.Models;

namespace SqlToCosmosFunction.Services
{
    public class SqlService
    {
        private readonly string _connectionString;
        public SqlService(string connectionString) => _connectionString = connectionString;

        public async Task<IEnumerable<Artist>> GetDataAsync(int batchSize)
        {
            var results = new List<Artist>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = GetQuery(1);
            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BatchSize", batchSize);

            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read()) 
            {
                var artist = new Artist
                {
                    Id = reader.GetInt32(0),
                    ArtistName = reader.GetString(1),
                    SortName = reader.GetString(2),
                    SearchName = reader.GetString(3),
                    Description = reader.GetString(4),
                    Genre = [reader.GetString(5)]
                };

                var existingArtist = results.FirstOrDefault(a => a.Id == artist.Id);
                if (existingArtist != null)
                {
                    existingArtist.Genre.AddRange(artist.Genre);
                }
                else
                {
                    results.Add(artist);
                }
            }

            return results;
        }

        private static string GetQuery(int batchSize)
        {
            return @"
                SELECT TOP (@batchSize) a.Id, a.ArtistName, a.SortName, a.SearchName, at.Description, g.GenreName
                    FROM Artist as a
                INNER JOIN ArtistGenre as ag on a.Id = ag.ArtistId
                INNER JOIN ArtistType at on a.ArtistTypeId = at.Id
                INNER JOIN Genre as g on ag.GenreId = G.Id
            ";
        }
    }
}