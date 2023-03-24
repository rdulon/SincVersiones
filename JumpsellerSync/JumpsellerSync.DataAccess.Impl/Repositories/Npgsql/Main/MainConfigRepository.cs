using AutoMapper;

using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Main
{
    public class MainConfigRepository : IMainConfigRepository
    {
        private static readonly MainConfiguration defaultConfig
            = new MainConfiguration { Jumpseller = new JumpsellerConfiguration() };
        private static readonly Type mainConfigurationType = typeof(MainConfiguration);

        private readonly MainNpgsqlDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<MainConfigRepository> logger;
        private MainConfiguration configCache;

        public MainConfigRepository(
            MainNpgsqlDbContext dbContext,
            IMapper mapper,
            ILogger<MainConfigRepository> logger)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        public BaseDbContext DbContext => dbContext;

        public async Task<DataAccessResult<JumpsellerConfiguration>> ReadJumpsellerAuthInfoAsync()
        {
            var header = "MainConfigurationRepository.ReadJumpsellerAuthInfo".AsLogHeader();
            try
            {
                logger.LogInformation($"{header} Init.");
                var config = await ReadConfigAsync();
                logger.LogInformation($"{header} End.");

                return DataAccessResult.Succeed(config.Jumpseller);
            }
            catch (Exception e)
            {
                logger.LogException(e, header);
                return DataAccessResult.Fail<JumpsellerConfiguration>();
            }
        }

        public async Task<DataAccessResult> ResetJumpsellerAuthInfoAsync()
        {
            var header = "MainConfigurationRepository.ResetAuthInfo".AsLogHeader();
            using var tx = dbContext.BeginTransaction();
            try
            {
                logger.LogInformation($"{header} Init.");
                var config = await ReadConfigAsync();
                config.Jumpseller = new JumpsellerConfiguration();
                await dbContext.SaveChangesAsync();
                await tx.CommitAsync();
                logger.LogInformation($"{header} End.");

                return DataAccessResult.Succeed();
            }
            catch (Exception e)
            {
                logger.LogException(e, header);
                return DataAccessResult.Fail();
            }
        }

        public async Task<DataAccessResult> SaveJumpsellerAuthorizationInfoAsync(JumpsellerConfiguration info)
        {
            var header = "MainConfigurationRepository.SaveJumpsellerAuthInfo".AsLogHeader();
            using var tx = dbContext.BeginTransaction();
            try
            {
                logger.LogInformation($"{header} Init.");
                var config = await ReadConfigAsync();

                mapper.Map(info, config.Jumpseller, mainConfigurationType, mainConfigurationType);
                var jcEntry = dbContext.ChangeTracker.Entries<JumpsellerConfiguration>().Single();
                jcEntry.State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                await tx.CommitAsync();
                logger.LogInformation($"{header} End.");

                return DataAccessResult.Succeed();

            }
            catch (Exception e)
            {
                await tx.RollbackAsync();
                logger.LogException(e, header);
                return DataAccessResult.Fail();
            }
        }

        private async Task<MainConfiguration> ReadConfigAsync()
        {
            configCache ??= await dbContext.MainConfiguration
                .AsQueryable()
                .FirstOrDefaultAsync();
            if (configCache == null)
            {
                configCache = defaultConfig;
                using var tx = dbContext.BeginTransaction();
                await dbContext.AddAsync(configCache);
                var i = await dbContext.SaveChangesAsync();
                if (i > 0)
                { await tx.CommitAsync(); }
                else
                {
                    await tx.RollbackAsync();
                    throw new Exception("Configuration couldn't be read.");
                }
            }
            return configCache;
        }
    }
}
