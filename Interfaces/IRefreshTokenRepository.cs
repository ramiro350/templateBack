using ArqPay.Models;

namespace ArqPay.Interfaces;

public interface IRefreshTokenRepository
{
  Task<RefreshToken> CreateRefreshToken(RefreshToken token);
  Task<RefreshToken> GetByToken(string token);
  void RevokeToken(Guid userId);
  IEnumerable<RefreshToken> GetByUserId(Guid userId);
} 