using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ArqPay.Controllers;
using ArqPay.Interfaces;
using ArqPay.Models.Requests;
using ArqPay.Models.Responses;
using FluentAssertions;

public class LoginControllerTests
{
    private readonly Mock<ILoginService> _loginServiceMock;
    private readonly LoginController _controller;

    public LoginControllerTests()
    {
        _loginServiceMock = new Mock<ILoginService>();
        _controller = new LoginController(_loginServiceMock.Object);
    }

    [Fact]
    public async Task Login_ValidRequest_ReturnsOkResultWithResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = "test@user.com", Password = "Password123" };
        var expectedResponse = new LoginResponse { AccessToken = "fake-access-token", RefreshToken = "fake-refresh-token", UserId = Guid.NewGuid() };
        _loginServiceMock.Setup(service => service.Login(loginRequest)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Login_ServiceThrowsException_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = "invalid@user.com", Password = "wrong-password" };
        _loginServiceMock.Setup(service => service.Login(loginRequest)).ThrowsAsync(new UnauthorizedAccessException("Usuário inválido"));

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("Usuário inválido");
    }

    [Fact]
    public async Task Authenticate_ValidRequest_ReturnsOkResultWithResponse()
    {
        // Arrange
        var authenticateRequest = new CreateRefreshTokenRequest { UserId = Guid.NewGuid()};
        var expectedResponse = new AuthenticateResponse { AccessToken = "new-access-token", RefreshToken = "new-refresh-token" };
        _loginServiceMock.Setup(service => service.Authenticate(authenticateRequest)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Authenticate(authenticateRequest);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Authenticate_ServiceThrowsException_ReturnsBadRequestResult()
    {
        // Arrange
        var authenticateRequest = new CreateRefreshTokenRequest { UserId = Guid.NewGuid() };
        _loginServiceMock.Setup(service => service.Authenticate(authenticateRequest)).ThrowsAsync(new Exception("Usuário não encontrado"));

        // Act
        var result = await _controller.Authenticate(authenticateRequest);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("Usuário não encontrado");
    }
}