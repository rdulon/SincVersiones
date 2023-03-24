using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl.Provider;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider
{
    public abstract class ProviderConfigRepository<TProduct, TConfiguration, TLogger>
        : IProviderConfigRepository<TConfiguration>
        where TProduct : ProviderProduct
        where TConfiguration : ProviderConfiguration
        where TLogger : class
    {
        protected readonly ProviderDbContext<TProduct, TConfiguration> dbContext;
        protected readonly ILogger<TLogger> logger;

        public ProviderConfigRepository(
            ProviderDbContext<TProduct, TConfiguration> dbContext,
            ILogger<TLogger> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public BaseDbContext DbContext => dbContext;

        public abstract TConfiguration DefaultConfig { get; }

        public async Task<DataAccessResult<TConfiguration>> ReadConfigAsync()
        {
            var header = "Configuration.ReadConfig".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var config = await dbContext
                    .Set<TConfiguration>()
                    .AsQueryable()
                    .FirstOrDefaultAsync();
                logger.LogInformation($"{header} End.");
                return DataAccessResult.Succeed(config);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<TConfiguration>();
        }

        public async Task<DataAccessResult> UpdateConfigAsync(TConfiguration config)
        {
            var header = "Configuration.UpdateConfig".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            IDbContextTransaction tx = null;
            try
            {
                var entry = dbContext.ChangeTracker
                    .Entries<TConfiguration>()
                    .Where(e => e.Entity.Id == config.Id)
                    .FirstOrDefault();
                if (entry == null)
                {
                    var dbConfigResult = await ReadConfigAsync();
                    entry = dbContext.Entry(config);
                    entry.State = dbConfigResult.OperationSucceed && dbConfigResult.Data == null
                        ? EntityState.Added
                        : dbConfigResult.OperationSucceed
                            ? EntityState.Modified
                            : throw new Exception("Couldn't read config from db.");
                }
                else { entry.State = EntityState.Modified; }

                using (tx = dbContext.BeginTransaction())
                {
                    await dbContext.SaveChangesAsync();
                    await tx.CommitAsync();
                    logger.LogInformation($"{header} End.");
                    return DataAccessResult.Succeed();
                }
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail();
        }
    }
}
