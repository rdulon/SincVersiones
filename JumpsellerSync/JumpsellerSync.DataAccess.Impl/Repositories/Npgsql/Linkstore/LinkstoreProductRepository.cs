using JumpsellerSync.DataAccess.Core.Repositories.Linkstore;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Linkstore
{
    public class LinkstoreProductRepository
        : ProviderProductRepository<LinkstoreProduct, LinkstoreBrand, LinkstoreConfiguration, LinkstoreProductRepository>,
        ILinkstoreProductRepository

    {
        public LinkstoreProductRepository(
            LinkstoreNpgsqlDbContext dbContext,
            ILogger<LinkstoreProductRepository> logger)
            : base(dbContext, logger)
        { }

    }
}
