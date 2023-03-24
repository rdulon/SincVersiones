using JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Tecnoglobal
{
    public class TecnoglobalProductRepository
        : ProviderProductRepository<TecnoglobalProduct, TecnoglobalBrand, TecnoglobalConfiguration, TecnoglobalProductRepository>,
          ITecnoglobalProductRepository

    {
        public TecnoglobalProductRepository(
            TecnoglobalNpgsqlDbContext dbContext,
            ILogger<TecnoglobalProductRepository> logger)
            : base(dbContext, logger)
        { }

    }
}
