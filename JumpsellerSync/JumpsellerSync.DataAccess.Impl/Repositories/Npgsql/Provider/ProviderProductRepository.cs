using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Core.QueryModels.Provider;
using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl.Main;
using JumpsellerSync.Domain.Impl.Provider;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider
{
    public abstract class ProviderProductRepository<TProduct, TBrand, TConfig, TLogger>
        : BaseRepository<TProduct, TLogger>,
          IProviderProductRepository<TProduct>
        where TProduct : ProviderProduct
        where TBrand : ProviderBrand
        where TConfig : ProviderConfiguration
        where TLogger : class
    {
        private readonly ProviderDbContext<TProduct, TConfig> dbContext;

        public ProviderProductRepository(
            ProviderDbContext<TProduct, TConfig> dbContext, ILogger<TLogger> logger)
            : base(dbContext, logger)
        {
            RepositoryName = typeof(TLogger).Name;
            this.dbContext = dbContext;
        }

        protected string RepositoryName { get; }

        public async Task<DataAccessResult<TProduct>> FindByRedcetusIdAsync(string redcetusId)
        {
            var header = $"{RepositoryName}.FindByRedcetusId".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = from product in dbContext.Products.AsQueryable()
                        where product.RedcetusProductId == redcetusId
                        select product;

                logger.LogInformation($"{header} End.");
                return DataAccessResult.Succeed(
                    await q.SingleOrDefaultAsync());
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<TProduct>();
        }

        public IAsyncEnumerable<TProduct> ReadSynchronizingToJumpsellerAsync()
        {
            return ReadAsync(p => p.RedcetusProductId != null, 0, -1);
        }
        public IAsyncEnumerable<TProduct> ReadNotIn(List<string> skus)
        {
            var header = "ProductRepository.ReadNotIn".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = from prod in dbContext.Products.AsQueryable()
                        where !skus.Contains(prod.ProductCode)
                        select prod;

                logger.LogInformation($"{header} End.");
                return q.AsAsyncEnumerable();
            }
            catch (System.Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<TProduct>().ToAsyncEnumerable();
        }

        public IAsyncEnumerable<TProduct> ReadAll()
        {
            var header = "ProductRepository.ReadNotIn".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = from prod in dbContext.Products.AsQueryable()
                        select prod;

                logger.LogInformation($"{header} End.");
                return q.AsAsyncEnumerable();
            }
            catch (System.Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<TProduct>().ToAsyncEnumerable();
        }

        public IAsyncEnumerable<TProduct> ReadUnsyncedProductsAsync(
            string skuOrName, string brandId, int offset, int limit, bool withStock)
        {
            var header = $"{RepositoryName}.ReadUnsyncedProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = GetUnsyncedProductsQuery(skuOrName, brandId, withStock);
                q = q.Skip(offset).Take(limit);

                logger.LogInformation($"{header} End.");
                return q.AsAsyncEnumerable();
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<TProduct>().ToAsyncEnumerable();
        }

        public async Task<int> GetUnsyncedProductPagesAsync(
            string skuOrName, string brandId, bool withStock, int limit)
        {
            var header = $"{RepositoryName}.GetUnsyncedProductPages".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = GetUnsyncedProductsQuery(skuOrName, brandId, withStock);
                var count = await q.CountAsync();
                var pages = (count / limit) + (count % limit == 0 ? 0 : 1);


                logger.LogInformation($"{header} Found {pages} pages.");
                return pages;
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return default;
        }

        public virtual IAsyncEnumerable<ProviderUnsyncedProductSuggestion> ReadUnsyncedSuggestionsAsync(
            string skuOrName, string brandId, int suggestionsLimit, bool withStock)
        {
            var header = $"{RepositoryName}.ReadUnsyncedSuggestions".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var pattern = $"%{skuOrName ?? ""}%";
                var q = from prod in dbContext.Products.AsQueryable()
                        join brand in dbContext.Set<TBrand>().AsQueryable()
                            on prod.BrandId equals brand.Id into r
                        from subBrand in r.DefaultIfEmpty()
                        where (brandId == null || subBrand.Id == brandId) &&
                               prod.RedcetusProductId == null &&
                               (EF.Functions.ILike(prod.Description, pattern) ||
                                EF.Functions.ILike(prod.ProductCode, pattern) ||
                                (subBrand != null &&
                                 EF.Functions.ILike(subBrand.Description, pattern))) &&
                               ((withStock && prod.Stock > 0) || !withStock)
                        orderby prod.Description
                        select new ProviderUnsyncedProductSuggestion
                        {
                            BrandName = subBrand != null ? subBrand.Description : null,
                            ProductDescription = prod.Description,
                            ProductId = prod.Id,
                            Sku = prod.ProductCode
                        };

                logger.LogInformation($"{header} End.");
                return q.Take(suggestionsLimit).AsAsyncEnumerable();
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<ProviderUnsyncedProductSuggestion>().ToAsyncEnumerable();
        }

        private IQueryable<TProduct> GetUnsyncedProductsQuery(string skuOrName, string brandId, bool withStock)
        {
            var pattern = $"%{skuOrName ?? ""}%";
            var q = from prod in dbContext.Products.AsQueryable()
                    join brand in dbContext.Set<TBrand>().AsQueryable()
                        on prod.BrandId equals brand.Id into r
                    from subBrand in r.DefaultIfEmpty()
                    where (brandId == null || subBrand.Id == brandId) &&
                           prod.RedcetusProductId == null &&
                           (EF.Functions.ILike(prod.Description, pattern) ||
                            EF.Functions.ILike(prod.ProductCode, pattern) ||
                            (subBrand != null &&
                             EF.Functions.ILike(subBrand.Description, pattern))) &&
                           ((withStock && prod.Stock > 0) || !withStock)
                    orderby prod.Description
                    select prod;
            return q;
        }

    }
}
