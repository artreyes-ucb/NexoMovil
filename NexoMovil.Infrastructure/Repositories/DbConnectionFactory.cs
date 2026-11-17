using Microsoft.Extensions.Configuration;
using MySqlConnector;
using NexoMovil.Core.Interfaces;
using System.Data;

namespace NexoMovil.Infrastructure.Repositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _mySqlConn;

        public DbConnectionFactory(IConfiguration config)
        {
            _mySqlConn = config.GetConnectionString("NexoMovilConnection") ?? string.Empty;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_mySqlConn);
        }
    }
}
