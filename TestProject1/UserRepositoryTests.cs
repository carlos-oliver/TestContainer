using FluentAssertions;
using TestProject.Repositories;
using TestProject1;
using Xunit;

namespace TestProject.Tests
{
    public class UserRepositoryTests : IClassFixture<SqlContainerFixture>
    {
        private readonly SqlContainerFixture _sqlContainerFixture;

        public UserRepositoryTests(SqlContainerFixture sqlContainerFixture)
        {
            _sqlContainerFixture = sqlContainerFixture;
        }

        [Fact]
        public async Task Should_Insert_And_Retrieve_User()
        {
            // Usando o método auxiliar para criar o repositório
            var userRepository = await TestContainerHelper.CreateRepositoryAsync(_sqlContainerFixture.SqlContainer);

            // Arrange
            var newUser = new User { Name = "Teste", Email = "teste@email.com" };

            // Act
            await userRepository.AddUserAsync(newUser);
            var userFromDb = await userRepository.GetUserByEmailAsync("teste@email.com");

            // Assert
            userFromDb.Should().NotBeNull();
            userFromDb!.Name.Should().Be("Teste");
        }

        // Outros testes podem reutilizar o método CreateRepositoryAsync da mesma forma
        [Fact]
        public async Task Should_Throw_Exception_When_User_Not_Found()
        {
            // Usando o método auxiliar para criar o repositório
            var userRepository = await TestContainerHelper.CreateRepositoryAsync(_sqlContainerFixture.SqlContainer);

            // Act
            var userFromDb = await userRepository.GetUserByEmailAsync("nonexistent@email.com");

            // Assert
            userFromDb.Should().BeNull();
        }
    }
}
