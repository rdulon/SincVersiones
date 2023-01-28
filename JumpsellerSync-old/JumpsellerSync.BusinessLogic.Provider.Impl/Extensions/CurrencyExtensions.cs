
using JumpsellerSync.BusinessLogic.Core.Services;
using JumpsellerSync.BusinessLogic.Provider.Impl.BCCh;
using JumpsellerSync.BusinessLogic.Provider.Impl.Options;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Extensions
{
    public static class CurrencyExtensions
    {
        public static IServiceCollection ConfigureFreeCurrencyConverter(
            this IServiceCollection services, IConfiguration config)
        {
            var options = config
                .GetSection(CurrencyOptions.CONFIG_SECTION)
                .Get<CurrencyOptions>();

            if (options.FreeCurrencyConverter == null)
            { throw new InvalidOperationException(nameof(CurrencyOptions.FreeCurrencyConverter)); }


            services.AddScoped<ICurrencyConverterService, FreeCurrencyConverterService>();
            services.AddHttpClient(
                FreeCurrencyConverterService.HTTP_CLIENT_NAME,
                client =>
                {
                    var uriBuilder = new UriBuilder(options.FreeCurrencyConverter.Url)
                    { Query = $"compact=ultra&apiKey={options.FreeCurrencyConverter.ApiKey}" };
                    client.BaseAddress = uriBuilder.Uri;
                });

            return services;
        }

        public static IServiceCollection ConfigureMockCurrencyConverter(
            this IServiceCollection services)
        {
            var factorEnv = Environment.GetEnvironmentVariable("STATIC_CURRENCY_FACTOR");
            var staticFactor = double.TryParse(factorEnv, out var v) ? v : 1;

            services.AddSingleton<ICurrencyConverterService, MockCurrencyConverter>(
                sp => new MockCurrencyConverter(staticFactor));

            return services;
        }

        public static IServiceCollection ConfigureBCChCurrencyConverter(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CurrencyOptions>(configuration.GetSection(CurrencyOptions.CONFIG_SECTION));
            services.AddScoped(
                sp => new SieteWSSoapClient(
                    SieteWSSoapClient.EndpointConfiguration.SieteWSSoap12));
            services.AddScoped<ICurrencyConverterService, BCChService>();

            return services;
        }
    }
}
