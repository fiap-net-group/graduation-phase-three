using Microsoft.EntityFrameworkCore.Storage;
using TechBlog.NewsManager.API.Domain.Database;
using TechBlog.NewsManager.API.Infrastructure.Database.Context;

namespace TechBlog.NewsManager.API.Infrastructure.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseContext _context;
        private IDbContextTransaction _transaction;

        public IBlogNewsRepository BlogNew { get; }

        public UnitOfWork(IDatabaseContext context, IBlogNewsRepository blogNew)
        {
            _context = context;
            BlogNew = blogNew;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
                throw new InvalidOperationException("Transaction already started");

            _transaction = await _context.BeginTransactionAsync(cancellationToken);
        }

        public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction not started");

            var response = await _context.CommitTransactionAsync(_transaction, cancellationToken);

            await _transaction.DisposeAsync();

            return response;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
                _transaction?.Dispose();
            }
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}