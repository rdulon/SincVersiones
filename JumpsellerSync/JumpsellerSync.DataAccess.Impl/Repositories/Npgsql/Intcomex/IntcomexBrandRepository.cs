using JumpsellerSync.DataAccess.Core.Repositories.Intcomex;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Intcomex;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Intcomex
{
    public class IntcomexBrandRepository
        : BaseRepository<IntcomexBrand, IntcomexBrandRepository>,
          IIntcomexBrandRepository
    {
        public IntcomexBrandRepository(
            IntcomexNpgsqlDbContext dbContext,
            ILogger<IntcomexBrandRepository> logger)
            : base(dbContext, logger)
        { }

    }
}
