using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Common.Util.Validators;

using Microsoft.Extensions.Logging;

using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static JumpsellerSync.BusinessLogic.Provider.Impl.Constants.ServiceConstants;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Services
{
    public class SynchronizeService : ISynchronizeService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ILogger<SynchronizeService> logger;
        private static readonly UrlValidator urlValidator = new UrlValidator();

        public SynchronizeService(
            string callbackUrl, string syncSessionId,
            IHttpClientFactory httpClientFactory,
            JsonSerializerOptions jsonSerializerOptions,
            ILogger<SynchronizeService> logger)
        {
            CallbackUrl = callbackUrl;
            SyncSessionId = syncSessionId;
            this.httpClientFactory = httpClientFactory;
            this.jsonSerializerOptions = jsonSerializerOptions;
            this.logger = logger;
        }

        protected string CallbackUrl { get; }

        protected string SyncSessionId { get; }

        public virtual async Task<ServiceResult> SynchronizeAsync(SynchronizationInfoDto syncInfo)
        {
            var header = "SynchronizeService.SyncProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var client = httpClientFactory.CreateClient(MAIN_HTTP_CLIENT);
            var requestUrl = new UriBuilder(CallbackUrl)
            {
                Query = $"syncSessionId={SyncSessionId}",
                Path = "/api/sync/products"
            }.Uri.ToString();

            var body = new StringContent(
                JsonSerializer.Serialize(syncInfo, jsonSerializerOptions),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            if (!urlValidator.Validate(requestUrl).IsValid)
            {
                logger.LogError($"{header} Invalid url \"{requestUrl}\".");
                return ServiceResult.Error();
            }

            try
            {
                using var resp = await client.PostAsync(requestUrl, body);
                resp.EnsureSuccessStatusCode();
                logger.LogInformation($"{header} End.");

                return ServiceResult.Succeed();
            }
            catch (HttpRequestException httpError)
            {
                logger.LogException(httpError, header);
                return ServiceResult.Error();
            }
        }
    }
}
