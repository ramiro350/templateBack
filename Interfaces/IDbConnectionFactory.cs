// ArqPay/Infrastructure/IDbConnectionFactory.cs
using System.Data;

namespace ArqPay.Infrastructure
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}