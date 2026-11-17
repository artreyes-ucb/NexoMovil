using Dapper;
using NexoMovil.Core.Interfaces;
using System.Data;
using System.Data.Common;

namespace NexoMovil.Infrastructure.Data
{
    public class DapperContext : IDapperContext
    {
        private readonly IDbConnectionFactory _connFactory;

        private static readonly AsyncLocal<(IDbConnection? Conn, IDbTransaction? Tx)>
            _ambient = new();

        public DapperContext(IDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        // UnitOfWork llamará esto al iniciar una transacción.
        public void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx)
        {
            _ambient.Value = (conn, tx);
        }

        // UnitOfWork llamará esto al finalizar o hacer rollback.
        public void ClearAmbientConnection()
        {
            _ambient.Value = (null, null);
        }

        /// <summary>
        /// Si el UnitOfWork está activo, usa la misma conexión y transacción.
        /// Si no, crea una conexión nueva desde IDbConnectionFactory.
        /// </summary>
        private (IDbConnection conn, IDbTransaction? tx, bool ownsConnection) GetConnAndTx()
        {
            var ambient = _ambient.Value;
            if (ambient.Conn != null)
                return (ambient.Conn, ambient.Tx, false);

            var conn = _connFactory.CreateConnection();
            return (conn, null, true);
        }

        private static async Task OpenIfNeededAsync(IDbConnection conn)
        {
            if (conn is DbConnection dbConn && dbConn.State == ConnectionState.Closed)
                await dbConn.OpenAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryAsync<T>(
                    new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                        await dbConn.CloseAsync();
                    conn.Dispose();
                }
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                        await dbConn.CloseAsync();
                    conn.Dispose();
                }
            }
        }

        public async Task<int> ExecuteAsync(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.ExecuteAsync(
                    new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                        await dbConn.CloseAsync();
                    conn.Dispose();
                }
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                var result = await conn.ExecuteScalarAsync(
                    new CommandDefinition(sql, param, tx, commandType: commandType));

                if (result == null || result == DBNull.Value)
                    return default!;

                return (T)Convert.ChangeType(result, typeof(T));
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                        await dbConn.CloseAsync();
                    conn.Dispose();
                }
            }
        }
    }
}
