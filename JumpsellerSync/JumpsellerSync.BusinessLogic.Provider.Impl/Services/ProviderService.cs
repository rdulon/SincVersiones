using AutoMapper;

using Hangfire;

using JumpsellerSync.BusinessLogic.Core;
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
    public abstract class ProviderService<TProduct, TBrand, TConfig, TLogger> : IProviderService
        where TProduct : ProviderProduct
        where TBrand : ProviderBrand
        where TConfig : ProviderConfiguration
        where TLogger : class

    {
        protected readonly ISynchronizeService.Factory synchronizeFactory;
        protected readonly IProviderProductRepository<TProduct> productRepository;
        protected readonly IBackgroundJobClient jobClient;
        protected readonly IMapper mapper;
        protected readonly ILogger<TLogger> logger;

        public ProviderService(
            ISynchronizeService.Factory synchronizeFactory,
            IProviderProductRepository<TProduct> productRepository,
            IBackgroundJobClient jobClient,
            IMapper mapper,
            ILogger<TLogger> logger)
        {
            this.synchronizeFactory = synchronizeFactory;
            this.productRepository = productRepository;
            this.jobClient = jobClient;
            this.mapper = mapper;
            this.logger = logger;
        }

        public virtual async Task SynchronizeProductsAsync(StartSynchronizationDto syncInfo)
        {
            // TODO: Persist information to be able to recover if 
            //       an unexpected event occurs

            var header = $"{ServiceName}.SynchronizeProducts".AsLogHeader();

            var jobId = Guid.NewGuid().ToString("N");
            logger.LogInformation($"{header} Init. Starting Job: {jobId}");

            jobClient.Schedule(() => SynchronizeProductsAsync(
                syncInfo.MainUrl, syncInfo.SyncSessionId, jobId), DateTimeOffset.Now);
            logger.LogInformation($"{header} Job started. End.");

            await Task.CompletedTask;
        }

        public virtual async Task SynchronizeProductsAsync(
            string callbackUrl, string syncSessionId, string jobId)
        {
            var header = $"{ServiceName}.SynchronizeProducts".AsLogHeader();

            var info = await GetSyncInfoAsync(header);
            var synchronizer = synchronizeFactory(callbackUrl, syncSessionId);

            await synchronizer.SynchronizeAsync(info);
        }

        protected virtual async Task<SynchronizationInfoDto> GetSyncInfoAsync(string logHeader)
        {
            var syncedProducts = Enumerable.Empty<SynchronizeProductDto>();
            var updated = await UpdateProductsAsync(logHeader);

            logger.LogInformation($"{logHeader} Provider synchronization finished.");
            if (updated)
            {
                productRepository.DbContext.DetachAll();
                var products = await productRepository.ReadSynchronizingToJumpsellerAsync().ToListAsync();
                syncedProducts = mapper.Map<IEnumerable<SynchronizeProductDto>>(products);

                LogSyncedProducts(logHeader, products);
            }

            logger.LogInformation(
                $"{logHeader} Synchronizing with main info of {syncedProducts.Count()} products.");

            return new SynchronizationInfoDto
            {
                SyncComplete = true,
                Products = syncedProducts
            };
        }

        protected abstract Task<bool> UpdateProductsAsync(string logHeader);

        public virtual async Task<ServiceResult<ProviderProductDetailsDto>> SynchronizeWithRedcetusAsync(
            string productId, string redcetusProductId)
        {
            var err = ServiceResult.Error<ProviderProductDetailsDto>(
                  $"No se ha podido sincronizar el producto \"{productId}\".");
            var header = $"{ServiceName}.SynchronizeWithRedcetus".AsLogHeader();

            logger.LogInformation($"{header} Init.");

            redcetusProductId = redcetusProductId?.Trim();
            redcetusProductId = redcetusProductId == "" ? default : redcetusProductId;
            if (redcetusProductId == null)
            {
                logger.LogInformation($"{header} Bad redcetus product id \"{redcetusProductId}\"");
                return ServiceResult.BadInput<ProviderProductDetailsDto>(
                    "Código de producto RedCetus es incorrecto.");
            }

            var readResult = await productRepository.ReadAsync(new[] { productId });
            if (!readResult.OperationSucceed)
            {
                logger.LogError($"{header} Error reading product.");
                return err;
            }

            var product = readResult.Data;
            if (product == null)
            {
                logger.LogInformation($"{header} Product not found.");
                return ServiceResult.NotFound<ProviderProductDetailsDto>();
            }

            if (product.RedcetusProductId != null &&
                product.RedcetusProductId != redcetusProductId)
            {
                logger.LogInformation(
                    $"{header} Product \"\" already linked to redcetus product: \"{product.RedcetusProductId}\"");
                return ServiceResult.BadInput<ProviderProductDetailsDto>(
                    "El producto ya se encuentra en sincronización.");
            }

            using var tx = productRepository.DbContext.BeginTransaction();
            try
            {
                if (product.RedcetusProductId == null)
                {
                    logger.LogInformation($"{header} Update product.");
                    product.RedcetusProductId = redcetusProductId;
                    var updateResult = await productRepository.UpdateAsync(product);
                    if (updateResult.OperationSucceed)
                    { await tx.CommitAsync(); }
                    else
                    { throw new Exception($"Update operation failed. ProductId: \"{productId}\""); }
                    logger.LogInformation($"{header} Update product successful.");
                }

                var productDto = mapper.Map<TProduct, ProviderProductDetailsDto>(product);
                logger.LogInformation($"{header} End.");

                return ServiceResult.Succeed(productDto);
            }
            catch (Exception e)
            {
                logger.LogException(e, header);
                await tx.RollbackAsync();
            }

            logger.LogInformation($"{header} Error.");
            return err;
        }

        protected string ServiceName => typeof(TLogger).Name;

        protected async Task Upsert<TModel>(
            IUpsertRepository<TModel> repo, IEnumerable<TModel> models,
            string logHeader, params string[] skipPropUpdate)
            where TModel : DomainModel
        {
            using var tx = repo.DbContext.BeginTransaction();
            try
            {
                await repo.UpsertAsync(models, false, skipProperitesUpdate: skipPropUpdate);
                await tx.CommitAsync();
            }
            catch (Exception e)
            {
                logger.LogException(e, logHeader);
                await tx.RollbackAsync();
            }
        }

        protected void LogSyncedProducts(string logHeader, List<TProduct> products)
        {
            var productsLog = new System.Text.StringBuilder();

            productsLog.AppendLine("********** Synced products **********");

            for (var i = 0; i < products.Count; i++)
            {
                var p = products[i];
                productsLog.AppendLine($"SyncProduct: {p.ProductCode}. RedcetusID: {p.RedcetusProductId}, " +
                    $"Price: {p.Price}, Stock: {p.Stock}, BrandId: {p.BrandId}.");
            }

            logger.LogInformation($"{logHeader} {productsLog}");
        }
    }
}

