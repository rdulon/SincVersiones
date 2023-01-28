using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Main
{
    public class ProviderRepository
        : BaseRepository<BaseProvider, ProviderRepository>,
          IProviderRepository
    {
        private readonly MainNpgsqlDbContext dbContext;

        public ProviderRepository(
            MainNpgsqlDbContext dbContext,
            ILogger<ProviderRepository> logger)
            : base(dbContext, logger)
        {
            this.dbContext = dbContext;
        }

        public IAsyncEnumerable<BaseProvider> ReadProvidersToSynchronize()
        {
            var now = DateTime.UtcNow;

            var q = from p in dbContext.Providers.AsNoTracking()
                    where p.NextSynchronization != null &&
                          p.NextSynchronization <= now &&
                          p.Active
                    orderby p.NextSynchronization, p.Priority
                    select p;

            return q.AsAsyncEnumerable();
        }
    }
}
