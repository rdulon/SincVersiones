using JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Tecnoglobal
{
    public class TecnoglobalBrandRepository
        : BaseRepository<TecnoglobalBrand, TecnoglobalBrandRepository>,
          ITecnoglobalBrandRepository
    {
        public TecnoglobalBrandRepository(
            TecnoglobalNpgsqlDbContext dbContext,
            ILogger<TecnoglobalBrandRepository> logger)
            : base(dbContext, logger)
        { }

    }
}
