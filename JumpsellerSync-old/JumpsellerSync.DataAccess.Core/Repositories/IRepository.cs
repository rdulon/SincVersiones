using JumpsellerSync.DataAccess.Core.DbContexts;

namespace JumpsellerSync.DataAccess.Core.Repositories
{
    public interface IRepository
    {
        public BaseDbContext DbContext { get; }
    }
}
