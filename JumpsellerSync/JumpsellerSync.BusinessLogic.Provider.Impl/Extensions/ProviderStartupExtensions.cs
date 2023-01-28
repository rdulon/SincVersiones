using Autofac;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Options;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.Common.Util.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;

using static JumpsellerSync.BusinessLogic.Provider.Impl.Constants.ServiceConstants;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Extensions
{
    public static class ProviderStartupExtensions
    {
        public static ContainerBuilder ConfigureBusinessLogicProviderContainer(
            this ContainerBuilder container, IConfiguration configuration)
        {
            container.RegisterType<SynchronizeService>()
                .As<ISynchronizeService>()
                .InstancePerLifetimeScope();

            return container;
        }


        public static IServiceCollection ConfigureBusinessLogicProviderServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient(MAIN_HTTP_CLIENT, client =>
                {
                    var mainAuth = configuration
                        .GetSection(MainAuthOptions.CONFIG_SECTION)
                        .Get<MainAuthOptions>();
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    if (env != Environments.Development &&
                        !string.IsNullOrEmpty(mainAuth.Username) &&
                        !string.IsNullOrEmpty(mainAuth.Password))
                    { client.AddBasicAuth(mainAuth.Username, mainAuth.Password); }
                });

            return services;
        }

    }
}
