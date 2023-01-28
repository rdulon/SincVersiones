using JumpsellerSync.DataAccess.Core.Repositories.Nexsys;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Nexsys;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Nexsys
{
    public class NexsysProductRepository
        : ProviderProductRepository<NexsysProduct, NexsysBrand, NexsysConfiguration, NexsysProductRepository>,
          INexsysProductRepository

    {
        public NexsysProductRepository(
            NexsysNpgsqlDbContext dbContext,
            ILogger<NexsysProductRepository> logger)
            : base(dbContext, logger)
        { }

    }
}
