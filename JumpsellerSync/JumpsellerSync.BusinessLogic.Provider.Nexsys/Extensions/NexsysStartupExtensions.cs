using Autofac;

using JumpsellerSync.BusinessLogic.Provider.Impl.Extensions;
using JumpsellerSync.BusinessLogic.Provider.Nexsys.DependencyInjection;
using JumpsellerSync.BusinessLogic.Provider.Nexsys.Options;
using JumpsellerSync.BusinessLogic.Provider.Nexsys.Services;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Impl.DbContexts;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JumpsellerSync.BusinessLogic.Provider.Nexsys.Extensions
{
    public static class NexsysStartupExtensions
    {
        public static ContainerBuilder ConfigureNexsysContainer(
            this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterInstance(
                new DbContextType
                {
                    ContextType = typeof(NexsysNpgsqlDbContext)
                }).SingleInstance();
            builder.AddDbContext<NexsysNpgsqlDbContext>(configuration, "Nexsys");
            builder.RegisterModule(new RepositoriesModule(configuration));
            builder.RegisterModule(new ServicesModule());

            return builder;
        }

        public static IServiceCollection ConfigureNexsysServices(
            this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            var nexsysOptions = configuration
                .GetSection(NexsysOptions.CONFIG_SECTION)
                .Get<NexsysOptions>();

            services
                .Configure<NexsysOptions>(configuration.GetSection(NexsysOptions.CONFIG_SECTION))
                .ConfigureBCChCurrencyConverter(configuration)
                .AddScoped(sp => new NexsysServiceSoapClient(
                        NexsysServiceSoapClient.EndpointConfiguration.NexsysServiceSoapPort))
                .AddSingleton(
                    new webServiceClient
                    {
                        country = nexsysOptions.Country,
                        username = nexsysOptions.Username
                    });

            return services;
        }
    }
}
