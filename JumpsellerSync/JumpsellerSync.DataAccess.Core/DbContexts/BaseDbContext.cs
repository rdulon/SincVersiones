using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using System.Linq;

namespace JumpsellerSync.DataAccess.Core.DbContexts
{
    public abstract class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options)
            : base(options)
        { }

        public IDbContextTransaction BeginTransaction()
        {
            IDbContextTransaction tx;
            try { tx = Database.BeginTransaction(); }
            catch { tx = new NoOpDbContextTransaction(); }

            return tx;
        }

        public void DetachAll()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in entries)
            {
                if (entry.Entity != null)
                {
                    entry.State = EntityState.Detached;
                }
            }
        }
    }

}
