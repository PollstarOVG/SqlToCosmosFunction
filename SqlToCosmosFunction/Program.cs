using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlToCosmosFunction.Interfaces;
using SqlToCosmosFunction.Services;
using static SqlToCosmosFunction.Options.DatabaseOptions;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;

        services.Configure<SqlDbSettings>(configuration.GetSection("SqlDb"));
        services.Configure<CosmosDbSettings>(configuration.GetSection("CosmosDb"));

        services.AddScoped<ISqlService, SqlService>();
        services.AddScoped<ICosmosService, CosmosService>();

        services.BuildServiceProvider().GetRequiredService<ISqlService>();
    })
    .Build();

host.Run();
