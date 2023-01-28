using JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Tecnoglobal
{
    public class TecnoglobalCategoryRepository
        : BaseRepository<TecnoglobalCategory, TecnoglobalCategoryRepository>,
          ITecnoglobalCategoryRepository
    {
        public TecnoglobalCategoryRepository(
            TecnoglobalNpgsqlDbContext dbContext,
            ILogger<TecnoglobalCategoryRepository> logger)
            : base(dbContext, logger)
        { }
    }
}
