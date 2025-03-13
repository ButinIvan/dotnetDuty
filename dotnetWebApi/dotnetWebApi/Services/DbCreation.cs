using Npgsql;

namespace dotnetWebApi.Services;

public class DbCreation
{
    private readonly string _connectionString;

    public DbCreation(EnvService envService)
    {
        _connectionString = envService.GetVariable("CONNECTION_STRING");
    }
    
    public async Task EnsureDatabaseExistsAsync()
    {
        Console.WriteLine("⏳ Checking environment variables...");
        foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
        {
            Console.WriteLine($"{envVar}: {Environment.GetEnvironmentVariable(envVar.ToString())}");
        }
        Console.WriteLine("✅ Environment variables loaded.");

        var defaultConnectionString = new NpgsqlConnectionStringBuilder(_connectionString).ConnectionString;

        var targetDbName = new NpgsqlConnectionStringBuilder(_connectionString).Database;
        
        Console.WriteLine($"Default Connection String: {defaultConnectionString}");
        Console.WriteLine($"Target Database: {targetDbName}");

        try
        {
            using var connection = new NpgsqlConnection(defaultConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{targetDbName}';";
            var dbExists = await command.ExecuteScalarAsync() != null;

            if (!dbExists)
            {
                command.CommandText = $"CREATE DATABASE \"{targetDbName}\";";
                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Database '{targetDbName}' created.");
            }
            else
            {
                Console.WriteLine($"Database '{targetDbName}' already exists.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw; // Повторно выбрасываем исключение для диагностики
        }
    }
}