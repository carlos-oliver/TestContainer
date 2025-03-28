using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.MsSql;

namespace IntegrationTests;

public class ApiIntegrationTests : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;
    private readonly HttpClient _client;

    public ApiIntegrationTests()
    {
        _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest") // Versão do SQL Server
            .WithPassword("YourStrong(!)Password") // Senha do SA
            .Build();

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                    builtConfig["ConnectionStrings:DefaultConnection"] = _sqlContainer.GetConnectionString();
                });
            });

        _client = application.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await InitializeDatabase();
    }

    private async Task InitializeDatabase()
    {
        using var connection = new SqlConnection(_sqlContainer.GetConnectionString());
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE Usuarios (
                Id INT PRIMARY KEY IDENTITY,
                Nome NVARCHAR(100) NOT NULL
            );

            INSERT INTO Usuarios (Nome) VALUES ('João'), ('Maria');";
        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.StopAsync();
    }

    [Fact]
    public async Task Deve_Retornar_Usuarios_Do_Banco()
    {
        var response = await _client.GetAsync("/api/usuarios");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("João", content);
        Assert.Contains("Maria", content);
    }
}

