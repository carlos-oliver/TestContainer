using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace TestProject.Tests;

public class MsSqlTestContainer
{
    public MsSqlContainer Container { get; }

    public MsSqlTestContainer()
    {
        Container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong!Passw0rd")
            .WithCleanUp(true)
            .Build();
    }

    public async Task StartAsync() => await Container.StartAsync();
    public async Task StopAsync() => await Container.StopAsync();
}
