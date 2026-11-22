using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;
using System.Data;

namespace NexoMovil.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NexoMovilContext _context;
        private readonly IDapperContext _dapper;

        private IProductRepository? _productRepository;
        private ISaleRepository? _saleRepository;
        private IUserRepository? _userRepository;
        private ISecurityRepository? _securityRepository;
        private IInvoiceRepository? _invoiceRepository;

        private IDbContextTransaction? _efTransaction;

        public UnitOfWork(NexoMovilContext context, IDapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public IProductRepository ProductRepository =>
            _productRepository ??= new ProductRepository(_context, _dapper);

        public ISaleRepository SaleRepository =>
            _saleRepository ??= new SaleRepository(_context, _dapper);

        public IUserRepository UserRepository =>
            _userRepository ??= new UserRepository(_context, _dapper);

        public ISecurityRepository SecurityRepository =>
            _securityRepository ??= new SecurityRepository(_context);

        public IInvoiceRepository InvoiceRepository =>
            _invoiceRepository ??= new InvoiceRepository(_context, _dapper);

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_efTransaction == null)
            {
                _efTransaction = await _context.Database.BeginTransactionAsync();

                var conn = _context.Database.GetDbConnection();
                var tx = _efTransaction.GetDbTransaction();
                _dapper.SetAmbientConnection(conn, tx);
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                if (_efTransaction != null)
                {
                    await _efTransaction.CommitAsync();
                    _efTransaction.Dispose();
                    _efTransaction = null;
                }
            }
            finally
            {
                _dapper.ClearAmbientConnection();
            }
        }

        public async Task RollbackAsync()
        {
            if (_efTransaction != null)
            {
                await _efTransaction.RollbackAsync();
                _efTransaction.Dispose();
                _efTransaction = null;
            }

            _dapper.ClearAmbientConnection();
        }

        public IDbConnection? GetDbConnection()
        {
            return _context.Database.GetDbConnection();
        }

        public IDbTransaction? GetDbTransaction()
        {
            return _efTransaction?.GetDbTransaction();
        }

        public void Dispose()
        {
            _efTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
