using JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Tecnoglobal
{
    public sealed class TecnoglobalConfigRepository
        : ProviderConfigRepository<TecnoglobalProduct, TecnoglobalConfiguration, TecnoglobalConfigRepository>,
          ITecnoglobalConfigRepository
    {
        public TecnoglobalConfigRepository(
            TecnoglobalNpgsqlDbContext dbContext,
            ILogger<TecnoglobalConfigRepository> logger)
            : base(dbContext, logger)
        { }

        public override TecnoglobalConfiguration DefaultConfig => new TecnoglobalConfiguration();
    }
}
