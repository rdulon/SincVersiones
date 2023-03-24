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
    public class NpgsqlProductRepository
        : BaseRepository<Product, NpgsqlProductRepository>,
          IProductRepository
    {
        private readonly MainNpgsqlDbContext dbContext;

        public NpgsqlProductRepository(
            MainNpgsqlDbContext dbContext, ILogger<NpgsqlProductRepository> logger)
            : base(dbContext, logger)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> CountProductsToSynchronizeAsync()
        {
            var q = from p in dbContext.Products.AsQueryable()
                    where !p.SynchronizedToJumpseller
                    select p;

            return await q.CountAsync();
        }

        public IAsyncEnumerable<Product> ReadAsync(
            string skuOrName, IEnumerable<string> brandIds, int offset, int limit)
        {
            var header = "ProductRepository.ReadAsync".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = GetProductsQuery(skuOrName, brandIds);

                logger.LogInformation($"{header} End.");
                return q
                    .OrderBy(prod => prod.Name)
                    .Skip(offset)
                    .Take(limit)
                    .AsAsyncEnumerable();
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<Product>().ToAsyncEnumerable();
        }

        public async Task<int> GetProductPagesAsync(
            string skuOrName, IEnumerable<string> brandIds, int limit)
        {
            var header = "ProductRepository.GetProductPages".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = GetProductsQuery(skuOrName, brandIds);
                var count = await q.CountAsync();
                var pages = (count / limit) + (count % limit == 0 ? 0 : 1);

                logger.LogInformation($"{header} Found {count} pages.");
                return pages;
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return default;
        }

        public async Task<DataAccessResult<Product>> ReadBySkuAsync(string sku)
        {
            var header = "ProductRepository.ReadBySku".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = from product in dbContext.Products.AsQueryable()
                        where product.SKU == sku
                        select product;

                logger.LogInformation($"{header} End.");
                return DataAccessResult.Succeed(await q.SingleOrDefaultAsync());
            }
            catch (System.Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<Product>();
        }

        public IAsyncEnumerable<Product> ReadProductsToSynchronizeAsync(int offset, int limit)
        {
            return ReadAsync(
                product => !product.SynchronizedToJumpseller, product => product.Id, offset, limit);
        }

        public IAsyncEnumerable<Product> ReadSkuOrNameSuggestionsAsync(string skuOrName, int suggestionsLimit)
        {
            var header = "ProductRepository.ReadSkuOrNameSuggestions".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var pattern = $"%{skuOrName ?? ""}%";
                var q = from prod in dbContext.Products.AsQueryable()
                        where EF.Functions.ILike(prod.Name, pattern) ||
                              EF.Functions.ILike(prod.SKU, pattern)
                        orderby prod.Name
                        select prod;

                logger.LogInformation($"{header} End.");
                return q.Take(suggestionsLimit).AsAsyncEnumerable();
            }
            catch (System.Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<Product>().ToAsyncEnumerable();
        }

        private IQueryable<Product> GetProductsQuery(string skuOrName, IEnumerable<string> brandIds)
        {
            skuOrName ??= "";
            var pattern = $"%{skuOrName}%";
            var brands = brandIds ?? Enumerable.Empty<string>();


            var q = skuOrName == "" && brands.Count() == 0
                ? dbContext.Products.AsQueryable()
                : skuOrName != "" && brands.Count() > 0
                    ? from prod in dbContext.Products.AsQueryable()
                      where (EF.Functions.ILike(prod.Name, pattern) ||
                             EF.Functions.ILike(prod.SKU, pattern)) &&
                            brands.Contains(prod.BrandId)
                      select prod
                    : from prod in dbContext.Products.AsQueryable()
                      where (skuOrName != "" && (EF.Functions.ILike(prod.Name, pattern) ||
                                                 EF.Functions.ILike(prod.SKU, pattern))) ||
                            (brands.Count() > 0 && brands.Contains(prod.BrandId))
                      select prod;
            return q;
        }
    }
}
