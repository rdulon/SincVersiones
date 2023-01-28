using JumpsellerSync.DataAccess.Core.Repositories.Intcomex;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Intcomex;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Intcomex
{
    public class IntcomexCategoryRepository
        : BaseRepository<IntcomexCategory, IntcomexCategoryRepository>,
          IIntcomexCategoryRepository
    {
        public IntcomexCategoryRepository(
            IntcomexNpgsqlDbContext dbContext,
            ILogger<IntcomexCategoryRepository> logger)
            : base(dbContext, logger)
        { }
    }
}
