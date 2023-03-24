using JumpsellerSync.DataAccess.Core.Repositories.Linkstore;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Linkstore
{
    public class LinkstoreCategoryRepository
        : BaseRepository<LinkstoreCategory, LinkstoreCategoryRepository>,
          ILinkstoreCategoryRepository
    {
        public LinkstoreCategoryRepository(
            LinkstoreNpgsqlDbContext dbContext,
            ILogger<LinkstoreCategoryRepository> logger)
            : base(dbContext, logger)
        { }
    }
}
