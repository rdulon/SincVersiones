using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.DbContexts
{
    public sealed class NoOpDbContextTransaction : IDbContextTransaction
    {
        public Guid TransactionId => Guid.NewGuid();

        public void Commit()
        { }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        { return Task.CompletedTask; }

        public void Dispose()
        { }

        public async ValueTask DisposeAsync()
        { await Task.CompletedTask; }

        public void Rollback()
        { }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        { return Task.CompletedTask; }
    }
}
