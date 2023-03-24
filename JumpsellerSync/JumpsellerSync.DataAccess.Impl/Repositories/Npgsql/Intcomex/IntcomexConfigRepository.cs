using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Core.Repositories.Intcomex;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Intcomex;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Intcomex
{
    public sealed class IntcomexConfigRepository
        : ProviderConfigRepository<IntcomexProduct, IntcomexConfiguration, IntcomexConfigRepository>,
          IIntcomexConfigRepository
    {
        public IntcomexConfigRepository(
            IntcomexNpgsqlDbContext dbContext,
            ILogger<IntcomexConfigRepository> logger)
            : base(dbContext, logger)
        { }

        public override IntcomexConfiguration DefaultConfig => new IntcomexConfiguration
        {
            LastCatalogUpdate = DateTime.MinValue
        };

        public async Task<DataAccessResult<DateTime>> GetLastCatalogUpdateAsync()
        {
            var header = "IntcomexConfig.GetLastCatalogUpdate".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var configResult = await ReadConfigAsync();
            var result = configResult.OperationSucceed
                ? DataAccessResult.Succeed(
                    configResult.Data?.LastCatalogUpdate ?? DateTime.MinValue)
                : DataAccessResult.Fail<DateTime>();
            logger.LogInformation($"{header} End.");
            return result;
        }

        public async Task<DataAccessResult> SetLastCatalogUpdateAsync(DateTime dt)
        {
            var header = "IntcomexConfig.UpdateLastCatalogUpdate".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var configResult = await ReadConfigAsync();
            var result = DataAccessResult.Fail();
            if (configResult.OperationSucceed)
            {
                var config = configResult.Data ?? DefaultConfig;
                config.LastCatalogUpdate = dt;
                result = await UpdateConfigAsync(config);
            }

            logger.LogInformation($"{header} End.");
            return result;
        }
    }
}
