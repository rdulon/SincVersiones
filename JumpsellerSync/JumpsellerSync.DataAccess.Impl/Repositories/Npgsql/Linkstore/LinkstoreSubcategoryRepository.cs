using JumpsellerSync.DataAccess.Core.Repositories.Linkstore;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Linkstore
{
    public class LinkstoreSubcategoryRepository
        : BaseRepository<LinkstoreSubcategory, LinkstoreSubcategoryRepository>,
          ILinkstoreSubcategoryRepository
    {
        public LinkstoreSubcategoryRepository(
            LinkstoreNpgsqlDbContext dbContext,
            ILogger<LinkstoreSubcategoryRepository> logger)
            : base(dbContext, logger)
        { }

    }
}
