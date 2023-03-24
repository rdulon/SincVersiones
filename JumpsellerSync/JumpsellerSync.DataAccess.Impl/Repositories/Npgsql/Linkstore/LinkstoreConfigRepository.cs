using JumpsellerSync.DataAccess.Core.Repositories.Linkstore;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.Extensions.Logging;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Linkstore
{
    public sealed class LinkstoreConfigRepository
        : ProviderConfigRepository<LinkstoreProduct, LinkstoreConfiguration, LinkstoreConfigRepository>,
          ILinkstoreConfigRepository
    {
        public LinkstoreConfigRepository(
            LinkstoreNpgsqlDbContext dbContext,
            ILogger<LinkstoreConfigRepository> logger)
            : base(dbContext, logger)
        { }

        public override LinkstoreConfiguration DefaultConfig => new LinkstoreConfiguration();
    }
}
