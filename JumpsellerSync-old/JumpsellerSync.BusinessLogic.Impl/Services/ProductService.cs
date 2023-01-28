using AutoMapper;

using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.BusinessLogic.Filter;
using JumpsellerSync.BusinessLogic.Impl.Extensions;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public class ProductService
        : BaseService<CreateProductDto, CreateProductDto, ProductDetailsDto, Product, ProductService>,
          IProductService
    {
        private readonly IProviderService providerService;
        private readonly IJumpsellerService jumpsellerService;
        private readonly IProductRepository productRepository;
        private readonly IBrandRepository brandRepository;
        private readonly IProviderRepository providerRepository;

        public ProductService(
            IProviderService providerService,
            IJumpsellerService jumpsellerService,
            IProductRepository productRepository,
            IBrandRepository brandRepository,
            IProviderRepository providerRepository,
            IMapper mapper,
            ILogger<ProductService> logger, DomainFilter<Product>.Factory factory)
            : base(mapper, logger, productRepository, productRepository,
                    productRepository, productRepository, factory)
        {
            this.providerService = providerService;
            this.jumpsellerService = jumpsellerService;
            this.productRepository = productRepository;
            this.brandRepository = brandRepository;
            this.providerRepository = providerRepository;
        }

        public override async Task<ServiceResult<ProductDetailsDto>> CreateAsync(CreateProductDto data)
        {
            var header = "ProductService.Create".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            if (string.IsNullOrEmpty(data.ProviderProductId))
            {
                logger.LogInformation($"{header} Invalid provider product id.");
                return ServiceResult.BadInput<ProductDetailsDto>();
            }

            var providerResult = await providerRepository.ReadAsync(new[] { data.ProviderId });
            if (providerResult.OperationSucceed)
            {
                if (providerResult.Data == null)
                {
                    logger.LogInformation($"{header} Provider \"{data.ProviderId}\" not found.");
                    return ServiceResult.NotFound<ProductDetailsDto>();
                }

                var productResult = await productRepository.ReadBySkuAsync(data.Sku);
                var createProductResult = await CreateProductAsync(data, productResult.Data);
                if (!createProductResult.IsSucceed())
                { return ServiceResult.Error<ProductDetailsDto>(createProductResult.Errors); }
                var product = createProductResult.Data;
                var syncProviderResult = await providerService.SynchronizeProductAsync(
                    data.ProviderId, data.ProviderProductId, product.Id);
                var syncJSResult = await jumpsellerService.SynchronizeProductAsync(product.Id);

                if (syncProviderResult.IsSucceed() && syncJSResult.IsSucceed())
                {
                    using var tx = productRepository.DbContext.BeginTransaction();
                    product.SynchronizingProviderIds.Add(data.ProviderId);
                    var updateResult = await productRepository.UpdateAsync(product);
                    if (updateResult.OperationSucceed)
                    {
                        await tx.CommitAsync();

                        logger.LogInformation($"{header} End.");
                        return ServiceResult.Succeed(
                            mapper.Map<ProductDetailsDto>(product));
                    }
                    await tx.RollbackAsync();
                }
            }

            logger.LogInformation(
                $"{header} Product not synced completely.");
            return ServiceResult.Error<ProductDetailsDto>(
                "El producto no ha sido sincronizado correctamente. Contacte " +
                "el administrador para que proceda manualmente. El proveedor es: " +
                $"{data.ProviderId}, el producto del proveedor es: {data.ProviderProductId} y " +
                $"el sku del producto es: {data.Sku}.");
        }

        public override async Task<ServiceResult> DeleteAsync(string productId)
        {
            var header = "ProductService.Delete".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var productResult = await productRepository.ReadAsync(new[] { productId });
            if (productResult.OperationSucceed)
            {
                var product = productResult.Data;
                if (product == null)
                {
                    logger.LogInformation($"{header} Product \"{productId}\" not found.");
                    return ServiceResult.NotFound();
                }

                var unsyncResult = await providerService.UnsynchronizeProductAsync(product.Id);
                if (unsyncResult.IsSucceed())
                {
                    var deleteResult = await productRepository.DeleteAsync(product);
                    logger.LogInformation($"{header} Product \"{productId}\" deleted? {deleteResult.YesNo()}");
                    logger.LogInformation($"{header} End.");
                    return deleteResult.ToServiceResult();
                }
            }

            logger.LogInformation($"{header} Product \"{productId}\" not deleted.");
            return ServiceResult.Error(
                "No se completó correctamente la operación de eliminación. " +
                "Contacte con el administrador para proceder manualmente. El " +
                $"producto es: \"{productId}\".");
        }

        public async Task<ServiceResult<IEnumerable<PrefetchedBrandDto>>> PrefetchBrandsAsync()
        {
            var header = "ProductService.PrefetchBrands".AsLogHeader();
            var result = Enumerable.Empty<PrefetchedBrandDto>();

            try
            {
                logger.LogInformation($"{header} Init.");
                var brands = await brandRepository
                    .ReadAsync(brand => true, brand => brand.Name, 0, -1)
                    .ToListAsync();

                result = mapper.Map<IEnumerable<PrefetchedBrandDto>>(brands);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            logger.LogInformation($"{header} End.");
            return ServiceResult.Succeed(result);
        }

        public async Task<PageResultDto<ProductDetailsDto>> ReadAsync(SearhProductsDto search)
        {
            var header = "ProductService.ReadAsync".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var offset = search.Page.GetDbOffset(search.Limit);
            var result = Enumerable.Empty<ProductDetailsDto>();
            var pages = 0;

            try
            {
                var productsResult = productRepository.ReadAsync(
                    search.SkuOrName, search.BrandIds, offset, search.Limit);
                result = mapper.Map<IEnumerable<ProductDetailsDto>>(
                    await productsResult.ToListAsync());
                pages = await productRepository.GetProductPagesAsync(
                    search.SkuOrName, search.BrandIds, search.Limit);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            logger.LogInformation($"{header} End.");
            return new PageResultDto<ProductDetailsDto>
            {
                Items = result,
                Limit = search.Limit,
                Page = search.Page,
                TotalPages = pages
            };
        }

        public async Task<IEnumerable<ProductSuggestionDto>> ReadProductSuggestionsAsync(
            ReadProductSuggestionsDto input)
        {
            var header = "ProductService.ProductSuggestion".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var products = Enumerable.Empty<ProductSuggestionDto>();
            try
            {
                var skuOrName = input.SkuOrName ?? "";
                var result =
                    await productRepository
                        .ReadSkuOrNameSuggestionsAsync(skuOrName, input.SuggestionsLimit)
                        .ToListAsync();
                products = mapper.Map<IEnumerable<ProductSuggestionDto>>(result);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            logger.LogInformation($"{header} End.");
            return products;
        }

        public override Task<ServiceResult<ProductDetailsDto>> UpdateAsync(CreateProductDto updatelDto)
        {
            throw new InvalidOperationException();
        }

        public async Task<ServiceResult> UpdateMarginAsync(ProductDetailsDto product)
        {
            var productResult = await productRepository.ReadAsync(new[] { product?.Id });
            if (!productResult.OperationSucceed)
            { return productResult.ToServiceResult(); }

            if (productResult.Data == null)
            { return ServiceResult.NotFound(); }

            var productDb = productResult.Data;
            productDb.Margin = product.Margin;

            using var tx = productRepository.DbContext.BeginTransaction();
            var updateResult = await productRepository.UpdateAsync(productDb);
            if (updateResult.OperationSucceed)
            {
                await tx.CommitAsync();
                return ServiceResult.Succeed();
            }
            await tx.RollbackAsync();
            return ServiceResult.Error();
        }

        private async Task<ServiceResult<Product>> CreateProductAsync(
            CreateProductDto data, Product product)
        {
            if (product == null)
            {
                var providerProductResult = await providerService
                    .ReadProductAsync(data.ProviderId, data.ProviderProductId);
                if (providerProductResult.IsSucceed())
                {
                    using var tx = productRepository.DbContext.BeginTransaction();
                    var brandError = ServiceResult.Error<Product>(
                        "Ha ocurrido un error procesando la marca del producto.");

                    product = mapper.Map<Product>(providerProductResult.Data);
                    product.Name = string.IsNullOrEmpty(data.Name)
                        ? product.Name
                        : data.Name;
                    product.Margin = data.Margin;

                    var upsertBrandResult = await brandRepository.UpsertAsync(new[] { product.Brand });
                    if (!upsertBrandResult.OperationSucceed)
                    { return brandError; }

                    var createResult = await productRepository.CreateAsync(product);
                    if (!createResult.OperationSucceed)
                    {
                        await tx.RollbackAsync();
                        return ServiceResult.Error<Product>(
                          "No se puede obtener el producto del proveedor.");
                    }

                    await tx.CommitAsync();
                }
            }

            return ServiceResult.Succeed(product);
        }
    }
}
