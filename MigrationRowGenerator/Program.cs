using System.Diagnostics.CodeAnalysis;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace MigrationRowGenerator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("appSettings.json")
            .AddJsonFile($"appSettings.{args.ElementAtOrDefault(0)}.json")
            .AddCommandLine(args)
            .Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddOptions<ProgramConfig>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection
            .AddLogging(c => c.AddSimpleConsole(options => options.IncludeScopes = true));

        await using var services = serviceCollection.BuildServiceProvider();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation(args.ElementAtOrDefault(0));
        logger.LogInformation(args.ElementAtOrDefault(1));
        var config = services.GetRequiredService<IOptions<ProgramConfig>>().Value;

        var databases = config.Databases?.ToList() ?? [];
        
        logger.LogInformation("config databases count={Count}", databases.Count);
        logger.LogInformation("database: name:{Name}, dbname:{DatabaseName}", databases[0].Name, databases[0].DatabaseName);
        if (!string.IsNullOrWhiteSpace(args.ElementAtOrDefault(1)))
        {
            databases = databases.Where(d => d.Name.Contains(args.ElementAtOrDefault(1))).ToList();
        }
        
        logger.LogInformation(databases.Count.ToString());
        foreach (var database in databases)
        {
            await using var connection = new MySqlConnection(database.ConnectionString);
            await connection.OpenAsync();

            var reader = await connection.ExecuteReaderAsync(config.Script);
            var rows = new List<string>();
            while (await reader.ReadAsync())
            {
                rows.Add(reader.GetString(0));
            }
            
            logger.LogInformation("DatabaseName={DatabaseName},  RowsCount={RowsCount}", database.DatabaseName, rows.Count);

            Splitter.GenerateFiles(database.DatabaseName, database.MigrationFileName+"koko", rows, 5000);
            logger.LogInformation("generated");
        }
    }
}

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal class ProgramConfig
{
    public string Script { get; set; } = null!;
    public Database[]? Databases { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
internal record Database(string Name, string ConnectionString, string MigrationFileName, string DatabaseName);
