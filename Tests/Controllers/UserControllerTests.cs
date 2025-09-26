using Moq;
using Microsoft.AspNetCore.Mvc;
using ArqPay.Controllers;
using ArqPay.Models;
using ArqPay.Models.Requests;
using ArqPay.Interfaces;
using FluentAssertions;
using Xunit;

namespace ArqPay.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _serviceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _serviceMock = new Mock<IUserService>();
            _controller = new UserController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetUserByEmail_ValidEmail_ReturnsOkResult()
        {
            // Arrange
            var request = new GetUserRequest { Email = "test@test.com" };
            var expectedUser = new User { Id = Guid.NewGuid(), Email = request.Email };
            _serviceMock.Setup(x => x.GetUser(request))
                       .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUser(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetUserByEmail_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var request = new GetUserRequest { Email = "test@test.com" };
            _serviceMock.Setup(x => x.GetUser(request))
                       .ThrowsAsync(new Exception("User not found"));

            // Act
            var result = await _controller.GetUser(request);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeEquivalentTo(new { error = "User not found" });
        }

        [Fact]
        public async Task CreateUser_ValidRequest_ReturnsOkWithId()
        {
            // Arrange
            var request = new CreateUserRequest { Name = "Test User", Email = "test@test.com" };
            var expectedId = Guid.NewGuid();
            _serviceMock.Setup(x => x.CreateUser(request))
                       .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(new { Id = expectedId });
        }

        [Fact]
        public async Task GetUserById_ValidId_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new User { Id = userId };
            _serviceMock.Setup(x => x.GetUserById(userId))
                       .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task UpdateUser_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new UpdateUserRequest { Id = Guid.NewGuid(), Name = "Updated Name" };
            _serviceMock.Setup(x => x.UpdateUser(request));

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Usuário atualizado");
        }

        [Fact]
        public async Task DeleteUser_ValidId_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _serviceMock.Setup(x => x.DeleteUser(userId))
                       .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Usuário excluído");
        }
    }
}