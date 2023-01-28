using Autofac;

using JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.DependencyInjection;
using JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Extensions;
using JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Options;
using JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Services;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Impl.DbContexts;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Extensions
{
    public static class TecnoglobalStartupExtensions
    {
        public static ContainerBuilder ConfigureTecnoglobalContainer(
            this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterInstance(
                new DbContextType
                {
                    ContextType = typeof(TecnoglobalNpgsqlDbContext)
                }).SingleInstance();
            builder.AddDbContext<TecnoglobalNpgsqlDbContext>(configuration, "Tecnoglobal");
            builder.RegisterModule(new RepositoriesModule(configuration));
            builder.RegisterModule(new ServicesModule());

            return builder;
        }

        public static IServiceCollection ConfigureTecnoglobalServices(
            this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.Configure<TecnoglobalOptions>(
                       configuration.GetSection(TecnoglobalOptions.CONFIG_SECTION))
                   .AddHttpClient(configuration);

            return services;
        }

        public static IServiceCollection AddHttpClient(
            this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration
                .GetSection(TecnoglobalOptions.CONFIG_SECTION)
                .Get<TecnoglobalOptions>();
            var api = options.Api;
            var auth = options.Auth;

            services.AddHttpClient(
                TecnoglobalProviderService.TECNOGLOBAL_HTTP_CLIENT,
                client =>
                {
                    client.BaseAddress = new Uri(api.BaseUrl);
                    client.AddBasicAuth(auth.Username, auth.Password);
                    client.DefaultRequestHeaders.Accept.Add(
                        MediaTypeWithQualityHeaderValue.Parse(
                            MediaTypeNames.Application.Json));
                });

            return services;
        }
    }
}
