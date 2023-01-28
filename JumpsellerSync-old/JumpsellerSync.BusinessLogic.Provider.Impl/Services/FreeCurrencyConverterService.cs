using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services;
using JumpsellerSync.Common.Util.Extensions;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Services
{
    public class FreeCurrencyConverterService : ICurrencyConverterService
    {
        public const string HTTP_CLIENT_NAME = "FreeCurrencyConverterHttpClientId";
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<FreeCurrencyConverterService> logger;

        public FreeCurrencyConverterService(
            IHttpClientFactory httpClientFactory,
            ILogger<FreeCurrencyConverterService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<ServiceResult<ConvertCurrencyDetailsDto>> ConvertAsync(string from, string to)
        {
            var header = "FreeCurrencyConverter.ConvertSimple".AsLogHeader();
            return FromConversion(
                await GetConversionAsync(from, to, header),
                from,
                to);
        }

        public async Task<ServiceResult<ConvertCurrencyDetailsDto>> ConvertAsync(
            string from, string to, double amount)
        {
            if (amount < 0)
            { return ServiceResult.Error<ConvertCurrencyDetailsDto>(); }

            var header = "FreeCurrencyConverter.ConvertAmount".AsLogHeader();
            return FromConversion(
                await GetConversionAsync(from, to, header),
                from,
                to,
                amount);
        }

        private ServiceResult<ConvertCurrencyDetailsDto> FromConversion(
            ServiceResult<double> conversionResult, string from, string to, double amount = 1)
        {
            return conversionResult.IsSucceed()
                ? ServiceResult.Succeed(new ConvertCurrencyDetailsDto
                {
                    From = from,
                    To = to,
                    Value = conversionResult.Data * amount
                })
                : ServiceResult.Error<ConvertCurrencyDetailsDto>();
        }

        private async Task<ServiceResult<double>> GetConversionAsync(string from, string to, string header)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                logger.LogInformation(
                    $"{header} Invalid conversion parameters (From: {from}, To: {to}).");
                return ServiceResult.Error<double>();
            }

            logger.LogInformation($"{header} Init.");

            try
            {
                var client = httpClientFactory.CreateClient(HTTP_CLIENT_NAME);
                var qv = $"{from.ToUpper()}_{to.ToUpper()}";
                var q = new NameValueCollection { ["q"] = qv };
                var uri = client.BaseAddress.Augment(default, q);
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var data = JsonSerializer.Deserialize<Dictionary<string, double>>(
                    await response.Content.ReadAsStringAsync());
                if (data.ContainsKey(qv))
                {
                    logger.LogInformation($"{header} End.");
                    return ServiceResult.Succeed(data[qv]);
                }
                logger.LogInformation($"{header} Conversion \"{qv}\" invalid or non-existent.");
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return ServiceResult.Error<double>();
        }
    }
}
