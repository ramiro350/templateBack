namespace ArqPay.Models

{
  public class AccessToken
  {
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public bool Expired { get; set; }  
  }
}
