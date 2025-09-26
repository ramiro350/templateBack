

namespace ArqPay.Models.Responses
{
  public class AuthenticateResponse
  {
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
    public bool Revoked { get; set; }
    public Guid UserId { get; set; }
  }
}