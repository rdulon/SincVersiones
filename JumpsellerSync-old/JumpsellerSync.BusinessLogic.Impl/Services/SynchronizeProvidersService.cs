using AutoMapper;

using Hangfire;

using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.BusinessLogic.Impl.Extensions;
using JumpsellerSync.BusinessLogic.Impl.Options;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public class SynchronizeProvidersService : ISynchronizeProvidersService
    {
        private readonly IBackgroundJobClient jobClient;
        private readonly ILogger<SynchronizeProvidersService> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IProviderRepository providerRepository;
        private readonly ISynchronizationSessionRepository sessionRepository;
        private readonly IProviderHelperService providerHelper;
        private readonly MainOptions mainOptions;
        private readonly IMapper mapper;

        public SynchronizeProvidersService(
            IBackgroundJobClient jobClient,
            ILogger<SynchronizeProvidersService> logger,
            IHttpClientFactory httpClientFactory,
            IProviderRepository providerRepository,
            ISynchronizationSessionRepository sessionRepository,
            IProviderHelperService providerHelper,
            IOptions<MainOptions> mainOptions,
            IMapper mapper)
        {
            this.jobClient = jobClient;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.providerRepository = providerRepository;
            this.sessionRepository = sessionRepository;
            this.providerHelper = providerHelper;
            this.mainOptions = mainOptions.Value;
            this.mapper = mapper;
        }

        public async Task<ServiceResult> SynchronizeProviderProductsAsync(
            string syncSessionId, SynchronizationInfoDto syncInfo)
        {
            var header = "SynchronizeProviders.SyncProduct".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var (sessionId, providerId) = providerHelper.UnwrapProviderSessionId(syncSessionId);
            var sessionResult = await sessionRepository.ReadAsync(new[] { sessionId, providerId });
            if (sessionResult.OperationSucceed && sessionResult.Data != null)
            {
                var session = sessionResult.Data;
                session.Running = !syncInfo.SyncComplete;
                session.Information.AddRange(
                    mapper.Map<IEnumerable<SynchronizeProductDto>,
                               IEnumerable<SynchronizationSessionInfo>>(
                        syncInfo.Products));

                using var tx = sessionRepository.DbContext.BeginTransaction();
                var updateResult = await sessionRepository.UpdateAsync(session);
                if (updateResult.OperationSucceed)
                { await tx.CommitAsync(); }
                else
                {
                    await tx.RollbackAsync();
                    logger.LogInformation(
                        $"{header} Update session (Session: {sessionId}, Provider: {providerId}) failed.");
                    return updateResult.ToServiceResult();
                }

                if (syncInfo.SyncComplete &&
                    await sessionRepository.IsSessionSynchronizedAsync(sessionId))
                {
                    logger.LogInformation(
                        $"{header} All providers finished synchronization. Starting reconcile.");
                    jobClient.Schedule<IReconcileProductsService>(
                        rp => rp.ReconcileSessionAsync(sessionId), DateTimeOffset.UtcNow);
                }

                logger.LogInformation($"{header} End.");
                return ServiceResult.Succeed();
            }

            if (sessionResult.Data == null)
            {
                logger.LogInformation(
                    $"{header} Sync session (Session: {sessionId}, Provider: {providerId}) not found.");
                return ServiceResult.NotFound();
            }
            return sessionResult.ToServiceResult();
        }

        public async Task<ServiceResult> SynchronizeProviderAsync(string providerId)
        {
            var header = "SynchronizeProviders.SyncOne".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var providerResult = await providerRepository.ReadAsync(new[] { providerId });
            if (!providerResult.OperationSucceed)
            { return providerResult.ToServiceResult(); }

            if (providerResult.Data == null)
            {
                logger.LogInformation(
                    $"{header} Provider with Id: \"{providerId}\" doesn't exist.");
                return ServiceResult.NotFound();
            }

            var processed = await ProcessProviderAsync(
                 providerResult.Data,
                 Guid.NewGuid().ToString(),
                 header);

            logger.LogInformation($"{header} End.");

            return processed
                ? ServiceResult.Succeed()
                : ServiceResult.Error("No fue posible iniciar la sincronización.");
        }

        public async Task SynchronizeProvidersAsync()
        {
            var header = $"SynchronizeProviders.SyncAll".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var providers = await providerRepository
                    .ReadProvidersToSynchronize()
                    .ToListAsync();
                var syncSessionId = Guid.NewGuid().ToString();

                foreach (var provider in providers)
                { await ProcessProviderAsync(provider, syncSessionId, header); }

                logger.LogInformation($"{header} end.");
            }
            catch (Exception e)
            { logger.LogException(e, header); }
        }

        private async Task<bool> ProcessProviderAsync(
            BaseProvider provider, string syncSessionId, string header)
        {
            var session = new SynchronizationSession
            {
                Id = syncSessionId,
                ProviderId = provider.Id,
                Running = true
            };

            var providerReachable =
                await providerHelper.IsProviderReachableAsync(provider.Url);
            logger.LogInformation(
                $"{header} Provider {provider.Name} is " +
                $"{(providerReachable ? "" : "not ")}reachable.");

            if (providerReachable)
            {
                var providerSessionId =
                    providerHelper.WrapProviderSessionId(syncSessionId, provider.Id);
                await SaveSessionAndStartSynchronizationAsync(
                    provider, session, providerSessionId, header);
            }

            return providerReachable;
        }

        private async Task SaveSessionAndStartSynchronizationAsync(
            BaseProvider provider, SynchronizationSession session,
            string providerSessionId, string header)
        {
            using var tx = sessionRepository.DbContext.BeginTransaction();
            try
            {
                var createSessionResult = await sessionRepository.CreateAsync(session);
                if (!createSessionResult.OperationSucceed)
                { throw new Exception(); }
                await tx.CommitAsync();

                var startSyncDto = new StartSynchronizationDto
                {
                    SyncSessionId = providerSessionId,
                    MainUrl = mainOptions.HostedUrl
                };

                // TODO: Handle whether the provider started
                //       synchronization or not
                await StartSynchronizationAsync(
                    provider, startSyncDto, header);
            }
            catch (Exception e)
            {
                logger.LogException(
                  e, $"{header} Save session for provider \"{provider.Name}\" failed");
                await tx.RollbackAsync();
            }
        }

        private async Task<bool> StartSynchronizationAsync(
            BaseProvider provider, StartSynchronizationDto startSyncDto, string logHeader)
        {
            var providerName = provider.Name;
            logger.LogInformation($"{logHeader} Start \"{providerName}\" synchronization.");
            try
            {
                var client = httpClientFactory.CreateClient();
                var uri = new UriBuilder(provider.Url)
                {
                    Path = "/api/sync"
                }.Uri;
                var body = new StringContent(
                    JsonSerializer.Serialize(startSyncDto),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json);

                using var response = await client.PostAsync(uri, body);
                response.EnsureSuccessStatusCode();

                logger.LogInformation(
                    $"{logHeader} \"{providerName}\" started synchronization.");
                return true;
            }
            catch (Exception e)
            {
                logger.LogException(
                    e, $"{logHeader} \"{providerName}\" failed to start synchronization");
            }

            return false;
        }
    }
}
