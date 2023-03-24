using JumpsellerSync.DataAccess.Core.Repositories.Nexsys;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Nexsys;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Nexsys
{
    public class NexsysCategoryRepository
        : BaseRepository<NexsysCategory, NexsysCategoryRepository>,
          INexsysCategoryRepository
    {
        public NexsysCategoryRepository(
            NexsysNpgsqlDbContext dbContext,
            ILogger<NexsysCategoryRepository> logger)
            : base(dbContext, logger)
        { }
    }
}
