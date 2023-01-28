using JumpsellerSync.DataAccess.Core.Repositories.Nexsys;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Nexsys;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Nexsys
{
    public class NexsysBrandRepository
        : BaseRepository<NexsysBrand, NexsysBrandRepository>,
          INexsysBrandRepository
    {
        public NexsysBrandRepository(
            NexsysNpgsqlDbContext dbContext,
            ILogger<NexsysBrandRepository> logger)
            : base(dbContext, logger)
        { }

    }
}
