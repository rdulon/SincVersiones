using Autofac;

using JumpsellerSync.BusinessLogic.Provider.Impl.Extensions;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.DependencyInjection;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Extensions;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Options;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Services;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Impl.DbContexts;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;

namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.Extensions
{
    public static class IntcomexStartupExtensions
    {
        public static ContainerBuilder ConfigureIntcomexContainer(
            this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterInstance(
                new DbContextType
                {
                    ContextType = typeof(IntcomexNpgsqlDbContext)
                }).SingleInstance();
            builder.AddDbContext<IntcomexNpgsqlDbContext>(configuration, "Intcomex");
            builder.RegisterModule(new RepositoriesModule(configuration));
            builder.RegisterModule(new ServicesModule());

            return builder;
        }

        public static IServiceCollection ConfigureIntcomexServices(
            this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.Configure<IntcomexOptions>(
                       configuration.GetSection(IntcomexOptions.CONFIG_SECTION))
                   .AddHttpClient(configuration);

            services.ConfigureBCChCurrencyConverter(configuration);
            return services;
        }

        public static IServiceCollection AddHttpClient(
            this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(IntcomexOptions.CONFIG_SECTION).Get<IntcomexOptions>();
            var api = options.Api;
            var auth = options.Auth;

            services.AddHttpClient(
                IntcomexProviderService.INTCOMEX_HTTP_CLIENT,
                client =>
                {
                    var signature = new StringBuilder();
                    using var sha256 = SHA256.Create();
                    var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    var signingKey = $"{auth.PublicKey},{auth.PrivateKey},{timestamp}";
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(signingKey));
                    foreach (var @byte in hashBytes)
                    { signature.Append(@byte.ToString("x2")); }
                    var bearerToken =
                        $"apiKey={auth.PublicKey}&" +
                        $"utcTimeStamp={timestamp}&" +
                        $"signature={signature}";

                    client.BaseAddress = new Uri(api.BaseUrl);
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", bearerToken);
                    client.DefaultRequestHeaders.Accept.Add(
                        MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));
                });

            return services;
        }
    }
}
