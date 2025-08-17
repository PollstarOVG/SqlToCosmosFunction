namespace SqlToCosmosFunction.Options
{
    public class DatabaseOptions
    {
        public class SqlDbSettings
        {
            public string ConnectionString { get; set; } = string.Empty;
        }

        public class CosmosDbSettings
        {
            public string Endpoint { get; set; } = string.Empty;
            public string AuthKey { get; set; } = string.Empty;
            public string DatabaseName { get; set; } = string.Empty;
            public string ContainerName { get; set; } = string.Empty;
        }
    }
}