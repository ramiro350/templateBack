using System.Data;
using Npgsql;

namespace ArqPay.Infrastructure
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _postgresConnectionString;

        public DbConnectionFactory(IConfiguration config)
        {
            _postgresConnectionString = config.GetConnectionString("Postgres")!;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_postgresConnectionString);
        }
    }
}
