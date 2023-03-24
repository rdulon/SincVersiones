using JumpsellerSync.DataAccess.Core.Repositories.Nexsys;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Nexsys;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Nexsys
{
    public sealed class NexsysConfigRepository
        : ProviderConfigRepository<NexsysProduct, NexsysConfiguration, NexsysConfigRepository>,
          INexsysConfigRepository
    {
        public NexsysConfigRepository(
            NexsysNpgsqlDbContext dbContext,
            ILogger<NexsysConfigRepository> logger)
            : base(dbContext, logger)
        { }

        public override NexsysConfiguration DefaultConfig => new NexsysConfiguration();
    }
}
