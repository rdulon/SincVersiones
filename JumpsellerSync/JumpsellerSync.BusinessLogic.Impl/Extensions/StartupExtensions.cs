using Autofac;

using Hangfire;

using JumpsellerSync.BusinessLogic.Core.Services;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.BusinessLogic.Filter;
using JumpsellerSync.BusinessLogic.Impl.DependencyInjection;
using JumpsellerSync.BusinessLogic.Impl.Options;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Impl.DbContexts;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

using static JumpsellerSync.BusinessLogic.Impl.Constants.HttpClientNames;

namespace JumpsellerSync.BusinessLogic.Impl.Extensions
{
    public static class StartupExtensions
    {
        public static ContainerBuilder ConfigureBusinessLogicContainer(
            this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterInstance(
                new DbContextType
                {
                    ContextType = typeof(MainNpgsqlDbContext)
                }).SingleInstance();
            builder.AddDbContext<MainNpgsqlDbContext>(configuration, "Jumpseller");
            builder.RegisterModule(new ServicesModule());
            builder.RegisterModule(new RepositoriesModule(configuration));
            builder.RegisterGeneric(typeof(DomainFilter<>)).InstancePerLifetimeScope();
            builder.RegisterType<ViewRenderService>()
                .As<IViewRenderService>()
                .InstancePerLifetimeScope();

            return builder;
        }

        public static IServiceCollection ConfigureBusinessLogicServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<JumpsellerOptions>(configuration.GetSection(JumpsellerOptions.CONFIG_SECTION))
                .Configure<MainOptions>(configuration.GetSection(MainOptions.CONFIG_SECTION))
                .AddHttpClient()
                .AddHttpClients(configuration);
        }

        public static IApplicationBuilder ConfigureBusinessLogicApp(this IApplicationBuilder app)
        {
            return app.StartCronJobs();
        }

        private static IApplicationBuilder StartCronJobs(this IApplicationBuilder app)
        {
            var recurringJobManager = app.ApplicationServices.GetService<IRecurringJobManager>();

            var cronExpr = Cron.Hourly();

            recurringJobManager.AddOrUpdate<ISynchronizeProvidersService>(
                "SynchronizeProvidersServiceImpl", sp => sp.SynchronizeProvidersAsync(), cronExpr);

            return app;
        }

        private static IServiceCollection AddHttpClients(
            this IServiceCollection services, IConfiguration configuration)
        {
            var jumpsellerOptions = configuration
                .GetSection(JumpsellerOptions.CONFIG_SECTION)
                .Get<JumpsellerOptions>();

            services
                .AddHttpClient(JUMPSELLER_AUTH_HTTP_CLIENT, client =>
                {
                    client.BaseAddress = new Uri(jumpsellerOptions.Auth.BaseUrl);
                });

            services
                .AddHttpClient(JUMPSELLER_API_HTTP_CLIENT, client =>
                {
                    var baseUri = new UriBuilder(jumpsellerOptions.Api.BaseUrl)
                    { Path = jumpsellerOptions.Api.Version }.Uri;
                    client.BaseAddress = baseUri;
                });
            services
                .AddHttpClient(PROVIDER_API_HTTP_CLIENT, client =>
                {
                });

            return services;
        }
    }
}
