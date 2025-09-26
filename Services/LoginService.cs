using ArqPay.Models;
using ArqPay.Repositories;
using ArqPay.Models.Requests;
using ArqPay.Helpers;
using System.Security.Cryptography;
using ArqPay.Interfaces;
using ArqPay.Models.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ArqPay.Services
{
  public class LoginService : ILoginService
  {
    private readonly IUserRepository _repository;
    public PasswordHelper _passwordHelper;
    public IRefreshTokenRepository _refreshTokenRepository;
    public IConfiguration _config;
    public IUserRepository _userRepository;

    public LoginService(IUserRepository repository, IRefreshTokenRepository refreshTokenRepository, IConfiguration config, IUserRepository userRepository)
    {
      _repository = repository;
      _passwordHelper = new PasswordHelper();
      _refreshTokenRepository = refreshTokenRepository;
      _config = config;
      _userRepository = userRepository;
    }

    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {

      User userResult = await _repository.GetUserWithCredentials(loginRequest.Email);

      if (loginRequest == null || !_passwordHelper.VerifyPassword(loginRequest.Password, userResult.passwordHash, userResult.salt))
      {
        throw new UnauthorizedAccessException("Usuário inválido");
      }

      IEnumerable<RefreshToken> refreshTokens = _refreshTokenRepository.GetByUserId(userResult.Id);

      foreach (RefreshToken Token in refreshTokens)
      {
        _refreshTokenRepository.RevokeToken(Token.Id);
      }

      var accessToken = GenerateJwtToken(loginRequest.Email);

      var expiresAt = DateTime.UtcNow.AddDays(10);

      var token = Guid.NewGuid().ToString();

      RefreshToken refreshToken = new RefreshToken()
      {
        Token = token,
        ExpiresAt = expiresAt,
        Revoked = false
      };

      _refreshTokenRepository.CreateRefreshToken(refreshToken);


      LoginResponse response = new LoginResponse()
      {
        AccessToken = accessToken,
        RefreshToken = token,
        UserId = userResult.Id
      };

      return response;
    }

    public async Task<AuthenticateResponse> Authenticate(CreateRefreshTokenRequest createRefreshTokenRequest)
      {
          IEnumerable<RefreshToken> refreshTokens = _refreshTokenRepository.GetByUserId(createRefreshTokenRequest.UserId);

          foreach (var token in refreshTokens)
          {
              if (token.ExpiresAt < DateTime.UtcNow)
              {
                _refreshTokenRepository.RevokeToken(token.Id);
              }
          }

          var user = await _userRepository.GetUserById(createRefreshTokenRequest.UserId);
          if (user is null)
          {
              throw new Exception("Usuário não encontrado");
          }

          var accessToken = GenerateJwtToken(user.Email); 
          var accessTokenExpires = DateTime.UtcNow.AddMinutes(60);

          var refreshTokenString = Guid.NewGuid().ToString();
          var refreshToken = new RefreshToken
          {
              Token = refreshTokenString,
              UserId = user.Id,
              ExpiresAt = DateTime.UtcNow.AddDays(10),
              Revoked = false
          };

          var createdRefreshToken = await _refreshTokenRepository.CreateRefreshToken(refreshToken);

          var response = new AuthenticateResponse
          {
              AccessToken = accessToken,
              AccessTokenExpiresAt = accessTokenExpires,
              RefreshToken = createdRefreshToken.Token,
              RefreshTokenExpiresAt = createdRefreshToken.ExpiresAt,
              UserId = user.Id
          };

          return response;
      }


    private string GenerateJwtToken(string username)
        {
            var jwtKey = _config["Jwt:Key"] ?? "my_local_dev_secret_key_123456789";
            var jwtIssuer = _config["Jwt:Issuer"] ?? "ArqPayAPI";
            var jwtAudience = _config["Jwt:Audience"] ?? "ArqPayClients";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
  }
}
