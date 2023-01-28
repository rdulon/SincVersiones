
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static JumpsellerSync.Common.Util.Services.ServiceUtilities;

namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public class ReconcileProductsService : IReconcileProductsService
    {
        private readonly ISynchronizationSessionRepository sessionRepository;
        private readonly IJumpsellerService jumpsellerService;
        private readonly IProductRepository productRepository;
        private readonly IProviderRepository providerRepository;
        private readonly ILogger<ReconcileProductsService> logger;
        private static readonly string[] reconcileSkipProductProperties
            = CreateSkipPropertiesArray<Product>(
                nameof(Product.Id), nameof(Product.Price), nameof(Product.Stock),
                nameof(Product.SynchronizedToJumpseller),
                nameof(Product.Categories), nameof(Product.Brand),
                nameof(Product.LocalProduct));

        public ReconcileProductsService(
            ISynchronizationSessionRepository sessionRepository,
            IJumpsellerService jumpsellerService,
            IProductRepository productRepository,
            IProviderRepository providerRepository,
            ILogger<ReconcileProductsService> logger)
        {
            this.sessionRepository = sessionRepository;
            this.jumpsellerService = jumpsellerService;
            this.productRepository = productRepository;
            this.providerRepository = providerRepository;
            this.logger = logger;
        }

        public async Task ReconcileSessionAsync(string sessionId)
        {
            var header = $"ReconcileProductsService.ReconcileSession".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            logger.LogInformation($"{header} Session: \"{sessionId}\"");

            var products = new List<Product>();

            await foreach (var reconcileInfo in
                sessionRepository.GetReconcileInformationAsync(sessionId))
            {
                logger.LogInformation(
                    $"{header} Product: \"{reconcileInfo.ProductId}\" updated from provider(s): " +
                    $"{string.Join(',', reconcileInfo.ProviderProducts.Select(ri => ri.ProviderId))}");

                products.Add(new Product
                {
                    Id = reconcileInfo.ProductId,
                    Price = reconcileInfo.ProviderProducts.Max(ri => ri.Price),
                    Stock = reconcileInfo.ProviderProducts.Sum(ri => ri.Stock),
                    SynchronizedToJumpseller = false
                });
            }

            using var tx = productRepository.DbContext.BeginTransaction();
            var updateResult = await productRepository.UpdateAsync(
                products, skipProperitesUpdate: reconcileSkipProductProperties);
            var processedEnded = await EndSyncSessionAsync(sessionId, header);

            if (updateResult.OperationSucceed && processedEnded)
            {
                await tx.CommitAsync();
                productRepository.DbContext.DetachAll();
                await jumpsellerService.SynchronizeProductsAsync();
                await jumpsellerService.SynchronizeLocalProductsAsync();
            }
            else
            {
                logger.LogInformation($"{header} Reconcile failed.");
                await tx.RollbackAsync();
            }
        }

        public async Task ReconcileSessionsAsync()
        {
            var header = "ReconcileProductsService.ReconcileSessionSynchronied".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var sessions = await sessionRepository
                .GetSynchronizedSessionsAsync()
                .ToListAsync();
            logger.LogInformation($"{header} Reconciling {sessions.Count} sessions.");
            foreach (var sessionId in sessions)
            { await ReconcileSessionAsync(sessionId); }

            logger.LogInformation($"{header} End.");
        }

        private async Task<bool> EndSyncSessionAsync(string sessionId, string logHeader)
        {
            var providers = await sessionRepository
                .ReadSessionProvidersAsync(sessionId)
                .ToListAsync();
            providers.ForEach(provider => provider.CalculateNextRun());

            var updateNextRunResult = await providerRepository.UpdateAsync(providers);
            var sessionProcessedResult = await sessionRepository.MarkSessionProcessedAsync(sessionId);

            if (updateNextRunResult.OperationSucceed && sessionProcessedResult.OperationSucceed)
            {
                logger.LogInformation($"{logHeader} Session \"{sessionId}\" finished.");

                return true;
            }

            logger.LogInformation($"{logHeader} Session \"{sessionId}\" couldn't finish.");

            return false;

        }
    }
}
