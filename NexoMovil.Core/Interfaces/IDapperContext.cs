using System.Data;

namespace NexoMovil.Core.Interfaces
{
    public interface IDapperContext
    {
        Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.Text);

        Task<T?> QueryFirstOrDefaultAsync<T>(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.Text);

        Task<int> ExecuteAsync(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.Text);

        Task<T> ExecuteScalarAsync<T>(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.Text);

        void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx);

        void ClearAmbientConnection();
    }
}
