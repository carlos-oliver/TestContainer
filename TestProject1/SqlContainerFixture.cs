using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace TestProject.Tests;

    public class SqlContainerFixture : IAsyncLifetime
    {
    public MsSqlContainer SqlContainer { get; private set; }

    public SqlContainerFixture()
    {
        // Configuração do container SQL Server
        SqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong!Passw0rd")
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Inicializa o contêiner
        await SqlContainer.StartAsync();
        await CreateDatabaseSchemaAsync();
    }

    public async Task DisposeAsync()
    {
        // Encerra o contêiner
        await SqlContainer.StopAsync();
    }

    // Método para criar a tabela Users e as stored procedures dentro do banco de dados
    private async Task CreateDatabaseSchemaAsync()
    {
        // String de conexão com o banco dentro do container
        var connectionString = SqlContainer.GetConnectionString();

        // Conectando ao banco de dados
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            // Comando SQL para criar a tabela Users
            var createUsersTable = @"
                    CREATE TABLE Users (
                        Id INT PRIMARY KEY IDENTITY,
                        Name NVARCHAR(100),
                        Email NVARCHAR(100) UNIQUE
                    )";

            // Comando SQL para criar a stored procedure de inserção
            var createInsertUserProc = @"
                    CREATE PROCEDURE sp_InsertUser
                        @Name NVARCHAR(100),
                        @Email NVARCHAR(100)
                    AS
                    BEGIN
                        INSERT INTO Users (Name, Email)
                        VALUES (@Name, @Email)
                    END";

            // Comando SQL para criar a stored procedure de consulta por e-mail
            var createGetUserByEmailProc = @"
                    CREATE PROCEDURE sp_GetUserByEmail
                        @Email NVARCHAR(100)
                    AS
                    BEGIN
                        SELECT * FROM Users WHERE Email = @Email
                    END";

            // Executa os comandos SQL para criar a tabela e as stored procedures
            using (var command = new SqlCommand(createUsersTable, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            using (var command = new SqlCommand(createInsertUserProc, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            using (var command = new SqlCommand(createGetUserByEmailProc, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
