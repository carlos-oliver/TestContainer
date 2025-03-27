using Testcontainers.MsSql;
using Microsoft.Extensions.Configuration;

namespace TestProject.Tests
{
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
        }

        public async Task DisposeAsync()
        {
            // Encerra o contêiner
            await SqlContainer.StopAsync();
        }
    }
}
