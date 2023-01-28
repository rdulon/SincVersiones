using Castle.Core.Internal;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Extensions;
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

        public async Task<IAsyncEnumerable<Product>> ReadAsync(
            string skuOrName, IEnumerable<string> brandIds, int offset, int limit, string provider = null)
        {
            var header = "ProductRepository.ReadAsync".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = GetProductsQuery(skuOrName, brandIds);

                logger.LogInformation($"{header} End.");

                var data = q
                    .OrderBy(prod => prod.Name);

                if (provider.IsNullOrEmpty())
                {
                    return data
                        .Skip(offset)
                        .Take(limit)
                        .AsAsyncEnumerable();
                }

                var list = await data.ToListAsync();

                return list.Where(prod => !prod.SynchronizingProviderIds.IsNullOrEmpty() && prod.SynchronizingProviderIds.Contains(provider))
                    .Skip(offset)
                    .Take(limit)
                    .ToAsyncEnumerable();
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<Product>().ToAsyncEnumerable();
        }

        public async Task<int> GetProductPagesAsync(
            string skuOrName, IEnumerable<string> brandIds, int limit, string provider = null)
        {
            var header = "ProductRepository.GetProductPages".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var q = GetProductsQuery(skuOrName, brandIds);
                int count = 0;

                if (provider.IsNullOrEmpty())
                {
                    count = await q.CountAsync();
                }
                else
                {
                    var data = q
                        .AsAsyncEnumerable();

                    if (provider != null)
                    {
                        var list = await data.ToListAsync();
                        count = list.Where(prod => !prod.SynchronizingProviderIds.IsNullOrEmpty() && prod.SynchronizingProviderIds.Contains(provider)).Count();
                    }
                    else
                    {
                        count = await data.CountAsync();
                    }
                }

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

            var q = skuOrName.IsNullOrEmpty() && brands.Count() == 0
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
