using Dapper;
using ArqPay.Infrastructure;
using ArqPay.Models;
using ArqPay.Interfaces;

namespace ArqPay.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<RefreshToken> CreateRefreshToken(RefreshToken token)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO refresh_tokens (refresh_token, expires_at, user_id, revoked)
                VALUES (@Token, @ExpiresAt, @UserId, @Revoked)
                RETURNING 
                    id AS Id,
                    refresh_token AS Token,
                    expires_at AS ExpiresAt,
                    user_id AS UserId,
                    revoked AS Revoked;
            ";
            return await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, token);
        }

        public async Task<RefreshToken?> GetByToken(string token)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    id AS Id,
                    refresh_token AS Token,
                    expires_at AS ExpiresAt,
                    user_id AS UserId,
                    revoked AS Revoked
                FROM refresh_tokens
                WHERE refresh_token = @token;
            ";
            return await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { token });
        }

        public async void RevokeToken(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE refresh_tokens 
                        SET revoked = true
                        WHERE id = @id;";
            await connection.ExecuteAsync(sql, new { id });
        }

        public async Task<int> DeleteByUserId(Guid UserId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM refresh_tokens WHERE user_id = @UserId;";
            return await connection.ExecuteAsync(sql, new { UserId });
        }

        public IEnumerable<RefreshToken> GetByUserId(Guid UserId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    id AS Id, 
                    refresh_token AS Token, 
                    expires_at AS ExpiresAt, 
                    user_id AS UserId, 
                    revoked AS Revoked
                FROM refresh_tokens
                WHERE user_id = @UserId
                  AND expires_at > NOW()
                  AND revoked = false;
            ";
            return connection.Query<RefreshToken>(sql, new { UserId });
        }
  }
}
