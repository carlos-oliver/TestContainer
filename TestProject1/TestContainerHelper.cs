using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using TestProject.Repositories;
using TestProject1;

namespace TestProject.Tests
{
    public static class TestContainerHelper
    {
        // Método para criar e configurar o DbContext e o UserRepository
        public static async Task<UserRepository> CreateRepositoryAsync(MsSqlContainer sqlContainer)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(sqlContainer.GetConnectionString())
                .Options;

            var dbContext = new AppDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Aplica as migrações

            return new UserRepository(dbContext);
        }
    }
}
