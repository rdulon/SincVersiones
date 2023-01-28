using JumpsellerSync.DataAccess.Core.Repositories.Linkstore;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Linkstore
{
    public class LinkstoreBrandRepository
        : BaseRepository<LinkstoreBrand, LinkstoreBrandRepository>,
          ILinkstoreBrandRepository
    {
        public LinkstoreBrandRepository(
            LinkstoreNpgsqlDbContext dbContext,
            ILogger<LinkstoreBrandRepository> logger)
            : base(dbContext, logger)
        { }

    }
}
