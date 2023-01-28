using AutoMapper;

using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
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

using static JumpsellerSync.Common.Util.Services.ServiceUtilities;

namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public class LocalProductService
        : BaseService<CreateLocalProductDto,
                      UpdateLocalProductDto,
                      LocalProductDetailsDto,
                      LocalProduct,
                      LocalProductService>,
          ILocalProductService
    {
        private static readonly string[] updateProductsSkipProperties
            = CreateSkipPropertiesArray<LocalProduct>(
                nameof(LocalProduct.Price), nameof(LocalProduct.Stock),
                nameof(LocalProduct.Brand), nameof(LocalProduct.Product));
        private readonly IVtexService vtexService;
        private readonly IProductRepository productRepository;
        private readonly ILocalProductRepository localProductRepository;

        public LocalProductService(
            IVtexService vtexService,
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<LocalProductService> logger,
            ILocalProductRepository localProductRepository,
            DomainFilter<LocalProduct>.Factory factory)
            : base(mapper,
                   logger,
                   default,
                   default,
                   localProductRepository,
                   localProductRepository,
                   factory)
        {
            this.vtexService = vtexService;
            this.productRepository = productRepository;
            this.localProductRepository = localProductRepository;
        }

        public async Task<ServiceResult> CreateAsync(IEnumerable<CreateLocalProductDto> localProductDtos)
        {
            var header = "LocalProductService.Create".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var dbIds = localProductDtos.Select(dto => dto.ProductId);
                var products = await productRepository
                    .ReadAsync(
                        p => dbIds.Contains(p.Id), 0, -1)
                    .ToListAsync();

                var localProducts = mapper
                    .Map<IEnumerable<LocalProduct>>(
                        products.Where(p => p.LocalProduct == null && p.JumpsellerId != null))
                    .ToDictionary(p => p.ProductId);
                SetInputPriceAndStock(localProductDtos, localProducts);

                if (localProducts.Count > 0)
                {
                    using var tx = localProductRepository.DbContext.BeginTransaction();
                    var insertResult = await localProductRepository.UpsertAsync(localProducts.Values);
                    if (!insertResult.OperationSucceed)
                    {
                        return ServiceResult.Error(
                            "Ha ocurrido un error desconocido mientras se guardaban los productos locales.");
                    }
                    await tx.CommitAsync();
                    await vtexService.SynchronizeLocalProductsAsync(
                        localProducts.Values.Select(lp => lp.Id));
                }

                logger.LogInformation($"{header} End.");
                return ServiceResult.Succeed();

            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return ServiceResult.Error(
                "Ha ocurrido un error desconocido mientras se creaban los productos locales.");
        }

        public async Task<IEnumerable<ProductDetailsDto>> SearchSyncedProductsAsync(
            string brandId, string skuOrName, int limit)
        {
            var header = "LocalProductService.SearchSyncedProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var result = Enumerable.Empty<ProductDetailsDto>();

            try
            {
                var productsResult
                    = localProductRepository.SearchSynchronizedProductsAsync(skuOrName, brandId, limit);
                result = mapper.Map<IEnumerable<ProductDetailsDto>>(
                    await productsResult.ToListAsync());
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            logger.LogInformation($"{header} End.");
            return result;
        }

        public async Task<ServiceResult> UpdateAsync(IEnumerable<UpdateLocalProductDto> localProducts)
        {
            var header = "LocalProductService.Update".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var updateProducts = mapper.Map<IEnumerable<LocalProduct>>(localProducts);
                var updateResult = await localProductRepository.UpsertAsync(
                    updateProducts, skipProperitesUpdate: updateProductsSkipProperties);

                logger.LogInformation($"{header} End.");
                return updateResult.ToServiceResult();
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return ServiceResult.Error("Ha ocurrido un error mientras se actualizaban los productos.");
        }

        private void SetInputPriceAndStock(
            IEnumerable<CreateLocalProductDto> localProductDtos,
            Dictionary<string, LocalProduct> localProducts)
        {

            foreach (var dto in localProductDtos)
            {
                if (localProducts.TryGetValue(dto.ProductId, out var product))
                {
                    product.Price = dto.Price ?? product.Price;
                    product.Stock = dto.Stock ?? product.Stock;
                }
            }
        }
    }
}
