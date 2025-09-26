namespace ArqPay.Models

{
  public class RefreshToken
  {
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public bool Revoked { get; set; }  
    public Guid AccessTokenId { get; set; }
  }
}
