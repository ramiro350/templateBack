

namespace ArqPay.Models.Responses
{
  public class LoginResponse
  {
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }
  }
}