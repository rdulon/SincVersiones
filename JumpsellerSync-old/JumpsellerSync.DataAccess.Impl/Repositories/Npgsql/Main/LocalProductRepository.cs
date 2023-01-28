using JumpsellerSync.Common.Util.Extensions;
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
    public class LocalProductRepository
        : BaseRepository<LocalProduct, LocalProductRepository>,
          ILocalProductRepository
    {
        private readonly MainNpgsqlDbContext dbContext;

        public LocalProductRepository(
            MainNpgsqlDbContext dbContext, ILogger<LocalProductRepository> logger)
            : base(dbContext, logger)
        {
            this.dbContext = dbContext;
        }

        public Task<int> CountAsync()
        {
            return dbContext
                    .LocalProducts
                    .AsQueryable()
                    .CountAsync();
        }

        public IAsyncEnumerable<Product> SearchSynchronizedProductsAsync(
            string skuOrName, string brandId, int limit)
        {
            var header = "LocalProductRepository.SearchSyncedProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                skuOrName ??= "";
                var pattern = $"%{skuOrName}%";
                brandId = string.IsNullOrEmpty(brandId) ? null : brandId;

                var q = from product in dbContext.Products.AsQueryable()
                        join local in dbContext.LocalProducts.AsQueryable()
                            on product.Id equals local.ProductId into localProducts
                        from localProduct in localProducts.DefaultIfEmpty()
                        where localProduct == null
                        select product;

                q = skuOrName != "" && brandId != null
                    ? q.Where(prod => (EF.Functions.ILike(prod.Name, pattern) ||
                                       EF.Functions.ILike(prod.SKU, pattern)) &&
                                      brandId == prod.BrandId)
                    : skuOrName != ""
                        ? q.Where(prod => EF.Functions.ILike(prod.Name, pattern) ||
                                                             EF.Functions.ILike(prod.SKU, pattern))
                        : q.Where(prod => brandId == prod.BrandId);

                logger.LogInformation($"{header} End.");
                return q
                    .OrderBy(prod => prod.Name)
                    .Take(limit)
                    .AsAsyncEnumerable();
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<Product>().ToAsyncEnumerable();
        }
    }
}
