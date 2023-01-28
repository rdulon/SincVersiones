using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.Common.Util.Extensions;

using Microsoft.Extensions.Logging;

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public class ProviderHelperService : IProviderHelperService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<ProviderHelperService> logger;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public ProviderHelperService(
            IHttpClientFactory httpClientFactory,
            ILogger<ProviderHelperService> logger,
            JsonSerializerOptions jsonSerializerOptions)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public string WrapProviderSessionId(string sessionId, string providerId)
            => Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{sessionId}:{providerId}"));

        public (string, string) UnwrapProviderSessionId(string providerSessionId)
        {
            providerSessionId = Encoding.UTF8.GetString(
                Convert.FromBase64String(providerSessionId ?? ""));

            var orig = providerSessionId?.Split(':');
            return orig?.Length == 2
                ? (orig[0], orig[1])
                : (default, default);
        }

        public async Task<bool> IsProviderReachableAsync(string providerUrl)
        {
            var header = "ProviderHelper.IsProviderReachable".AsLogHeader();

            try
            {
                logger.LogInformation($"{header} Init.");
                var client = httpClientFactory.CreateClient();
                var uri = new UriBuilder(providerUrl)
                {
                    Path = "/api/ping"
                }.Uri;
                using var request = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var msg = await response.Content.ReadAsStringAsync();
                msg = JsonSerializer.Deserialize<string>(msg, jsonSerializerOptions);
                if (string.Compare("pong", msg, true) != 0)
                { throw new Exception(); }

                logger.LogInformation(
                    $"{header} Provider located at \"{providerUrl}\" is reachable.");
                return true;
            }
            catch (Exception e)
            { logger.LogException(e, $"{header} Provider unreachable"); }

            return false;
        }
    }
}
