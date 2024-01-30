using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Logger;

namespace TechBlog.NewsManager.API.Infrastructure.Database.Context
{
    public sealed class SqlServerContext : DbContext, IDatabaseContext
    {
        private readonly ILoggerManager _logger;
        public DbSet<BlogNew> BlogNew { get; set; }

        public SqlServerContext(DbContextOptions<SqlServerContext> options, ILoggerManager logger) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
            _logger = logger;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqlServerContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> AnyPendingMigrationsAsync()
        {
            try
            {
                var migrations = await Database.GetPendingMigrationsAsync();

                return migrations.Any();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task MigrateAsync()
        {
            await Database.MigrateAsync();
        }

        public async Task TestConnectionAsync()
        {
            try
            {
                _ = await Database.ExecuteSqlRawAsync("SELECT 1");
            }
            catch (Exception ex)
            {
                _logger.LogException("Invalid database connection", LoggerManagerSeverity.CRITICAL, ex, ("context", nameof(SqlServerContext)));
                Environment.Exit(2);
            }
        }

        public async new Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await Database.BeginTransactionAsync(cancellationToken);
        }

        public Task<bool> CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
        {
            try
            {
                transaction.Commit();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogException("Error commiting transaction", LoggerManagerSeverity.ERROR, ex);
                transaction.Rollback();
                return Task.FromResult(false);
            }
        }
    }
}
