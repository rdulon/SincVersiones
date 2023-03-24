using AutoMapper;

using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories;
using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl;
using JumpsellerSync.Domain.Impl.Provider;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Services
{
    public abstract class ProviderProductService<TProduct, TBrand, TCategory, TLogger>
        : IProviderProductService
        where TProduct : ProviderProduct
        where TBrand : ProviderBrand
        where TCategory : ProviderCategory
        where TLogger : class
    {
        private readonly IProviderProductRepository<TProduct> productRepository;
        private readonly IReadRepository<TBrand> readBrandRepository;
        private readonly IReadRepository<TCategory> readCategoryRepository;
        private readonly IMapper mapper;
        private readonly ILogger<TLogger> logger;

        protected abstract string[] SkuSkipUpdateProperties { get; }

        public ProviderProductService(
            IProviderProductRepository<TProduct> productRepository,
            IReadRepository<TBrand> readBrandRepository,
            IReadRepository<TCategory> readCategoryRepository,
            IMapper mapper,
            ILogger<TLogger> logger)
        {
            this.productRepository = productRepository;
            this.readBrandRepository = readBrandRepository;
            this.readCategoryRepository = readCategoryRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public virtual async Task<ServiceResult<ProviderBrandDetailsDto>> ReadBrandAsync(
            string brandId)
        {
            var header = $"{ServiceName}.ReadBrand".AsLogHeader();
            return
                await ReadAsync<ProviderBrandDetailsDto, TBrand>(
                    brandId, header, readBrandRepository);
        }

        public virtual async Task<IEnumerable<ProviderBrandDetailsDto>> ReadBrandsAsync(
            int page, int limit)
        {
            var header = $"{ServiceName}.ReadBrands".AsLogHeader();

            var offset = page.GetDbOffset(limit);
            try
            {
                logger.LogInformation($"{header} Init.");
                var readBrandsResult = await readBrandRepository
                    .ReadAsync(brand => true, brand => brand.Description, offset, limit)
                    .ToListAsync();
                logger.LogInformation($"{header} End.");

                return mapper.Map<IEnumerable<TBrand>, IEnumerable<ProviderBrandDetailsDto>>(readBrandsResult);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<ProviderBrandDetailsDto>();
        }

        public virtual async Task<ServiceResult<ProviderProductDetailsDto>> ReadProductAsync(
            string productId)
        {
            var header = $"{ServiceName}.ReadProduct".AsLogHeader();
            return
                await ReadAsync<ProviderProductDetailsDto, TProduct>(
                    productId, header, productRepository);
        }

        public virtual async Task<PageResultDto<ProviderProductDetailsDto>> ReadUnsyncedProductsAsync(
            SearchProviderUnsyncedProductsDto search)
        {
            var header = $"{ServiceName}.ReadUnsyncedProducts".AsLogHeader();
            var offset = search.Page.GetDbOffset(search.Limit);
            var result = Enumerable.Empty<ProviderProductDetailsDto>();
            var totalPages = 0;
            try
            {
                logger.LogInformation($"{header} Init.");
                var skuOrName = search.SkuOrName ?? "";
                var products = productRepository.ReadUnsyncedProductsAsync(
                    skuOrName, search.BrandId, offset, search.Limit, search.WithStock);
                totalPages = await productRepository.GetUnsyncedProductPagesAsync(
                    skuOrName, search.BrandId, search.WithStock, search.Limit);

                result = mapper.Map<IEnumerable<ProviderProductDetailsDto>>(
                    await products.ToListAsync());
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            logger.LogInformation($"{header} End.");
            return new PageResultDto<ProviderProductDetailsDto>
            {
                Items = result,
                Page = search.Page,
                Limit = search.Limit,
                TotalPages = totalPages
            };
        }

        public virtual async Task<ServiceResult<ProviderCategoryDetailsDto>> ReadCategoryAsync(
            string categoryId)
        {
            var header = $"{ServiceName}.ReadCategory".AsLogHeader();
            return
                await ReadAsync<ProviderCategoryDetailsDto, TCategory>(
                    categoryId, header, readCategoryRepository);
        }

        public virtual async Task<IEnumerable<ProviderCategoryDetailsDto>> ReadCategoriesAsync(
            int page, int limit)
        {
            var header = $"{ServiceName}.ReadCategories".AsLogHeader();
            var offset = page.GetDbOffset(limit);
            try
            {
                logger.LogInformation($"{header} Init.");
                var categories = await readCategoryRepository
                    .ReadAsync(cat => true, cat => cat.Id, offset, limit)
                    .ToListAsync();
                logger.LogInformation($"{header} End.");

                return mapper.Map<IEnumerable<ProviderCategoryDetailsDto>>(categories);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<ProviderCategoryDetailsDto>();
        }

        public async Task<ServiceResult> SynchronizeProductAsync(
            string productId, string redcetusId)
        {
            var header = $"{ServiceName}.SynchronizeProduct".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            if (string.IsNullOrEmpty(redcetusId))
            {
                logger.LogInformation($"{header} Invalid main product id.");
                return ServiceResult.BadInput();
            }

            var productResult = await productRepository.ReadAsync(new[] { productId });
            if (productResult.OperationSucceed)
            {
                var product = productResult.Data;
                if (product == null)
                {
                    logger.LogInformation($"{header} Product \"{productId}\" not found.");
                    return ServiceResult.NotFound();
                }

                product.RedcetusProductId = redcetusId;
                return await UpdateProductAsync(product, header);
            }

            return ServiceResult.Error();
        }

        public async Task<ServiceResult> UnsynchronizeProductAsync(string redcetusId)
        {
            var header = $"{ServiceName}.UnsynchronizeProduct".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var productResult = await productRepository.FindByRedcetusIdAsync(redcetusId);
            if (productResult.OperationSucceed)
            {
                var product = productResult.Data;
                if (product != null)
                {
                    product.RedcetusProductId = null;
                    return await UpdateProductAsync(product, header);
                }

                return ServiceResult.Succeed();
            }

            return ServiceResult.Error();
        }

        public async Task<ServiceResult<IEnumerable<ProviderBrandInfo>>> SynchronizeProductsBySkuAsync(
            SynchronizeSkuDto info)
        {
            var header = $"{ServiceName}.SyncProductsBySku".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var map = info.SkuInfo.ToDictionary(i => i.Sku, i => i.RedcetusId);
                var skus = map.Keys.ToList();
                var productsDb = await productRepository
                    .ReadAsync(
                        p => skus.Contains(p.ProductCode), 0, -1)
                    .ToListAsync();
                if (productsDb.Count > 0)
                {
                    using var tx = productRepository.DbContext.BeginTransaction();
                    productsDb.ForEach(p => p.RedcetusProductId = map[p.ProductCode]);
                    var updateResult = await productRepository.UpdateAsync(
                        productsDb, skipProperitesUpdate: SkuSkipUpdateProperties);

                    logger.LogInformation($"{header} End.");
                    if (updateResult.OperationSucceed)
                    {
                        await tx.CommitAsync();
                        var result = await GetBrandInfoAsync(productsDb, header);
                        return ServiceResult.Succeed(result);
                    }
                    await tx.RollbackAsync();
                }
                return ServiceResult.Succeed(Enumerable.Empty<ProviderBrandInfo>());
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return ServiceResult.Error<IEnumerable<ProviderBrandInfo>>();
        }

        public async Task<ServiceResult<IEnumerable<ProductSuggestionDto>>> ReadUnsyncedProductSuggestionsAsync(
            ReadUnsyncedProductSuggestionsDto input)
        {
            var header = $"{ServiceName}.ReadUnsyncedProductSuggestions".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var result = Enumerable.Empty<ProductSuggestionDto>();
            try
            {
                var skuOrName = input.SkuOrName ?? "";
                var suggestions = productRepository.ReadUnsyncedSuggestionsAsync(
                    skuOrName, input.BrandId, input.SuggestionsLimit, input.WithStock);
                result = mapper.Map<IEnumerable<ProductSuggestionDto>>(
                    await suggestions.ToListAsync());
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            logger.LogInformation($"{header} End.");
            return ServiceResult.Succeed(result);
        }

        private async Task<IEnumerable<ProviderBrandInfo>> GetBrandInfoAsync(
            List<TProduct> productsDb, string header)
        {
            try
            {
                var brandInfo = productsDb
                        .Where(p => p.BrandId != null)
                        .GroupBy(p => p.BrandId)
                        .ToDictionary(g => g.Key, g => g.Select(p => p.RedcetusProductId));
                var brands = await readBrandRepository.ReadAsync(
                    brand => brandInfo.Keys.Contains(brand.Id),
                    0, -1).ToListAsync();

                var result = brands.Select(brand => new ProviderBrandInfo
                {
                    Brand = mapper.Map<ProviderBrandDetailsDto>(brand),
                    RedcetusIds = brandInfo[brand.Id]
                });
                return result;
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<ProviderBrandInfo>();
        }

        protected string ServiceName => typeof(TLogger).Name;

        private async Task<ServiceResult<TDto>> ReadAsync<TDto, TModel>(
            string id, string header, IReadRepository<TModel> repo)
            where TModel : DomainModel
        {
            try
            {
                logger.LogInformation($"{header} Init.");
                var result = await repo.ReadAsync(new[] { id });
                if (result.OperationSucceed)
                {
                    if (result.Data == null)
                    {
                        logger.LogInformation($"{header} Id \"{id}\" not found.");
                        return ServiceResult.NotFound<TDto>();
                    }

                    logger.LogInformation($"{header} End.");
                    return ServiceResult.Succeed(
                        mapper.Map<TDto>(result.Data));
                }
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return ServiceResult.Error<TDto>(
                     "No se puede acceder a la base de datos en estos momentos.");
        }

        private async Task<ServiceResult> UpdateProductAsync(TProduct product, string header)
        {
            using var tx = productRepository.DbContext.BeginTransaction();
            var updateResult = await productRepository.UpdateAsync(product);
            if (updateResult.OperationSucceed)
            {
                logger.LogInformation($"{header} End.");
                await tx.CommitAsync();
                return ServiceResult.Succeed();
            }
            await tx.RollbackAsync();
            return ServiceResult.Error();
        }
    }
}
