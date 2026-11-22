using System.Data;

namespace NexoMovil.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository ProductRepository { get; }
        ISaleRepository SaleRepository { get; }
        IUserRepository UserRepository { get; }
        ISecurityRepository SecurityRepository { get; }
        IInvoiceRepository InvoiceRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        IDbConnection? GetDbConnection();
        IDbTransaction? GetDbTransaction();
    }
}
