using Moq;
using ArqPay.Models;
using ArqPay.Models.Requests;
using ArqPay.Interfaces;
using ArqPay.Services;
using ArqPay.Helpers;
using FluentAssertions;
using Xunit;

namespace ArqPay.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly PasswordHelper _passwordHelper;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _passwordHelper = new PasswordHelper();
            _service = new UserService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateUser_ValidRequest_ReturnsUserId()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Name = "Test User",
                Email = "test@test.com",
                Password = "password123",
                Cpf = "12345678901",
                DataNascimento = new DateTime(1990, 1, 1),
                Nacionalidade = "Brazilian",
                Naturalidade = "São Paulo",
                Sexo = "M"
            };

            var expectedUserId = Guid.NewGuid();
            _repositoryMock.Setup(x => x.CreateUser(It.IsAny<User>()))
                         .ReturnsAsync(expectedUserId);

            // Act
            var result = await _service.CreateUser(request);

            // Assert
            result.Should().Be(expectedUserId);
            _repositoryMock.Verify(x => x.CreateUser(It.Is<User>(u => 
                u.Name == request.Name && 
                u.Email == request.Email &&
                !string.IsNullOrEmpty(u.passwordHash) &&
                !string.IsNullOrEmpty(u.salt))), Times.Once);
        }

        [Fact]
        public async Task CreateUser_InvalidCpf_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Cpf = "invalid-cpf",
                Email = "test@test.com",
                DataNascimento = new DateTime(1990, 1, 1)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUser(request));
        }

        [Fact]
        public async Task CreateUser_InvalidEmail_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Cpf = "12345678901",
                Email = "invalid-email",
                DataNascimento = new DateTime(1990, 1, 1)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUser(request));
        }

        [Fact]
        public async Task CreateUser_FutureBirthDate_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Cpf = "12345678901",
                Email = "test@test.com",
                DataNascimento = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUser(request));
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new User { Id = userId, Name = "Test User" };
            _repositoryMock.Setup(x => x.GetUserById(userId))
                         .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserById(userId);

            // Assert
            result.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetUserById_UserNotExists_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _repositoryMock.Setup(x => x.GetUserById(userId))
                         .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetUserById(userId));
        }

        [Fact]
        public async Task UpdateUser_ValidRequest_UpdatesUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId, Name = "Old Name", Email = "old@test.com" };
            var request = new UpdateUserRequest { Id = userId, Name = "New Name" };

            _repositoryMock.Setup(x => x.GetUserById(userId))
                         .ReturnsAsync(existingUser);
            _repositoryMock.Setup(x => x.GetUserWithCredentials(existingUser.Email))
                         .ReturnsAsync(new User { passwordHash = "hash", salt = "salt" });
            _repositoryMock.Setup(x => x.UpdateUser(It.IsAny<User>()))
                         .ReturnsAsync(1);

            // Act
            _service.UpdateUser(request);

            // Assert
            _repositoryMock.Verify(x => x.UpdateUser(It.Is<User>(u => 
                u.Id == userId && u.Name == "New Name")), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserExists_DeletesUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId };
            _repositoryMock.Setup(x => x.GetUserById(userId))
                         .ReturnsAsync(existingUser);
            _repositoryMock.Setup(x => x.DeleteUser(userId))
                         .ReturnsAsync(1);

            // Act
            var result = await _service.DeleteUser(userId);

            // Assert
            result.Should().Be(1);
            _repositoryMock.Verify(x => x.DeleteUser(userId), Times.Once);
        }
    }
}