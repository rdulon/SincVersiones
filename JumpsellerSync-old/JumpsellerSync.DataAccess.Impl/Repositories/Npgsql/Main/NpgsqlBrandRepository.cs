using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Main
{
    public class NpgsqlBrandRepository
        : BaseRepository<Brand, NpgsqlBrandRepository>,
          IBrandRepository
    {
        private readonly MainNpgsqlDbContext dbContext;

        public NpgsqlBrandRepository(
            MainNpgsqlDbContext dbContext,
            ILogger<NpgsqlBrandRepository> logger)
            : base(dbContext, logger)
        {
            this.dbContext = dbContext;
        }

        public async Task<DataAccessResult<int>> CreateAsync(IEnumerable<Brand> brands, bool deferSave = false)
        {
            var header = "BrandRepository.Create".AsLogHeader();
            try
            {
                DbContext.AddRange(brands.ToArray());
                var insertCount = 0;
                if (!deferSave)
                {
                    logger.LogInformation($"{header} Save changes.");
                    insertCount = await DbContext.SaveChangesAsync();
                }
                logger.LogInformation($"{header} End.");

                return DataAccessResult.Succeed(insertCount);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<int>();
        }

        public async Task<DataAccessResult<Brand>> FindBrandByNormalizedNameAsync(string normalizedName)
        {
            var header = "BrandRepository.FindBrandByName".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = from brand in dbContext.Brands.AsQueryable()
                        where brand.NormalizedName == normalizedName
                        select brand;

                var brandDb = await q.SingleOrDefaultAsync();

                logger.LogInformation($"{header} End.");
                return DataAccessResult.Succeed(brandDb);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<Brand>();
        }


    }
}
