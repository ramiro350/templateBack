using Xunit;
using Moq;
using ArqPay.Services;
using ArqPay.Interfaces;
using ArqPay.Models.Requests;
using ArqPay.Models.Responses;
using ArqPay.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using ArqPay.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LoginServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly LoginService _service;
    private readonly Mock<PasswordHelper> _passwordHelperMock;

    public LoginServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _configMock = new Mock<IConfiguration>();
        _passwordHelperMock = new Mock<PasswordHelper>();

        // Configure IConfiguration mock
        _configMock.SetupGet(c => c["Jwt:Key"]).Returns("my_local_dev_secret_key_123456789");
        _configMock.SetupGet(c => c["Jwt:Issuer"]).Returns("ArqPayAPI");
        _configMock.SetupGet(c => c["Jwt:Audience"]).Returns("ArqPayClients");

        _service = new LoginService(
            _userRepositoryMock.Object, 
            _refreshTokenRepositoryMock.Object, 
            _configMock.Object, 
            _userRepositoryMock.Object 
        );

        // Use the mocked password helper to control password verification
        _service._passwordHelper = _passwordHelperMock.Object;
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = "test@user.com", Password = "1234" };
        var user = new User { Id = Guid.NewGuid(), Email = "test@user.com", passwordHash = "3/oMcjobqss9voHNwBgFSHmIA4297oOWwRMhw84W86E=", salt = "4JMX46n3OJUfIFjY8OfBug==" };
        var existingTokens = new List<RefreshToken> { new RefreshToken { Id = Guid.NewGuid(), Token = "old_token", UserId = user.Id } };

        _userRepositoryMock.Setup(repo => repo.GetUserWithCredentials(loginRequest.Email)).ReturnsAsync(user);
        _refreshTokenRepositoryMock.Setup(repo => repo.GetByUserId(user.Id)).Returns(existingTokens);
        _refreshTokenRepositoryMock.Setup(repo => repo.RevokeToken(It.IsAny<Guid>()));
        _refreshTokenRepositoryMock.Setup(repo => repo.CreateRefreshToken(It.IsAny<RefreshToken>())).ReturnsAsync(new RefreshToken { Token = "new_token", UserId = user.Id });

        // Act
        var result = await _service.Login(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.UserId.Should().Be(user.Id);

        _refreshTokenRepositoryMock.Verify(repo => repo.RevokeToken(existingTokens[0].Id), Times.Once);
    }

    [Fact]
    public async Task Login_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = "test@user.com", Password = "WrongPassword" };
        var user = new User { Id = Guid.NewGuid(), Email = "test@user.com", passwordHash = "hashed", salt = "salt" };

        _userRepositoryMock.Setup(repo => repo.GetUserWithCredentials(loginRequest.Email)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Login(loginRequest));
    }

    [Fact]
    public async Task Authenticate_ValidRequest_ReturnsAuthenticateResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createRefreshTokenRequest = new CreateRefreshTokenRequest { UserId = userId };
        var user = new User { Id = userId, Email = "test@user.com" };
        var existingTokens = new List<RefreshToken>
        {
            new RefreshToken { Id = Guid.NewGuid(), Token = "old_valid_token", ExpiresAt = DateTime.UtcNow.AddDays(1), UserId = userId },
            new RefreshToken { Id = Guid.NewGuid(), Token = "expired_token", ExpiresAt = DateTime.UtcNow.AddDays(-1), UserId = userId }
        };
        var newRefreshToken = new RefreshToken { Token = "new-token", UserId = userId, ExpiresAt = DateTime.UtcNow.AddDays(10) };

        _refreshTokenRepositoryMock.Setup(repo => repo.GetByUserId(userId)).Returns(existingTokens);
        _refreshTokenRepositoryMock.Setup(repo => repo.RevokeToken(It.IsAny<Guid>()));
        _userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
        _refreshTokenRepositoryMock.Setup(repo => repo.CreateRefreshToken(It.IsAny<RefreshToken>())).ReturnsAsync(newRefreshToken);

        // Act
        var result = await _service.Authenticate(createRefreshTokenRequest);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().Be(newRefreshToken.Token);
        result.UserId.Should().Be(userId);

        _refreshTokenRepositoryMock.Verify(repo => repo.RevokeToken(existingTokens[1].Id), Times.Once); // Verify expired token is revoked
        _refreshTokenRepositoryMock.Verify(repo => repo.CreateRefreshToken(It.Is<RefreshToken>(rt => rt.UserId == userId)), Times.Once);
    }

    [Fact]
    public async Task Authenticate_UserNotFound_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createRefreshTokenRequest = new CreateRefreshTokenRequest { UserId = userId};

        _refreshTokenRepositoryMock.Setup(repo => repo.GetByUserId(userId)).Returns(new List<RefreshToken>());
        _userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.Authenticate(createRefreshTokenRequest));
        exception.Message.Should().Be("Usuário não encontrado");
    }
}