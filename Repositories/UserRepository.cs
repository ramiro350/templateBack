using Dapper;
using ArqPay.Infrastructure;
using ArqPay.Models;
using ArqPay.Interfaces;

namespace ArqPay.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    id AS Id, 
                    name AS Name, 
                    email AS Email, 
                    cpf AS Cpf, 
                    data_nascimento AS DataNascimento, 
                    sexo AS Sexo,
                    nacionalidade AS Nacionalidade,
                    naturalidade AS Naturalidade,
                    created_at AS CreatedAt, 
                    updated_at AS UpdatedAt
                FROM users
                WHERE email = @Email;
            ";

            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User?> GetUserById(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    id AS Id, 
                    name AS Name, 
                    email AS Email, 
                    cpf AS Cpf, 
                    data_nascimento AS DataNascimento, 
                    sexo AS Sexo, 
                    nacionalidade AS Nacionalidade,
                    naturalidade AS Naturalidade, 
                    created_at AS CreatedAt, 
                    updated_at AS UpdatedAt
                FROM users
                WHERE id = @Id;
            ";

            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    id AS Id, 
                    name AS Name, 
                    email AS Email, 
                    cpf AS Cpf, 
                    data_nascimento AS DataNascimento, 
                    sexo AS Sexo, 
                    nacionalidade AS Nacionalidade,
                    naturalidade AS Naturalidade,
                    created_at AS CreatedAt, 
                    updated_at AS UpdatedAt
                FROM users;
            ";

            return await connection.QueryAsync<User>(sql);
        }

        public async Task<Guid> CreateUser(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO users 
                    (name, email, password_hash, salt, cpf, data_nascimento, nacionalidade, naturalidade, sexo, created_at, updated_at) 
                VALUES 
                    (@Name, @Email, @PasswordHash, @Salt, @Cpf, @DataNascimento, @Nacionalidade, @Naturalidade, @sexo, NOW(), NOW())
                RETURNING id;
            ";

            return await connection.ExecuteScalarAsync<Guid>(sql, user);
        }

        public async Task<int> UpdateUser(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE users
                SET 
                    name = @Name,
                    email = @Email,
                    password_hash = @PasswordHash,
                    salt = @Salt,
                    cpf = @Cpf,
                    data_nascimento = @DataNascimento,
                    sexo = @Sexo,
                    naturalidade = @Naturalidade,
                    nacionalidade = @Nacionalidade,
                    updated_at = NOW()
                WHERE id = @Id;
            ";

            return await connection.ExecuteAsync(sql, user);
        }

        public async Task<int> DeleteUser(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM users WHERE id = @Id;";
            return await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<User?> GetUserWithCredentials(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    id AS Id, 
                    email AS Email,
                    cpf AS Cpf, 
                    password_hash AS PasswordHash, 
                    salt AS Salt
                FROM users
                WHERE email = @Email OR cpf = @Email;
            ";

            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }
    }
}
