using Autofac;

using JumpsellerSync.BusinessLogic.Provider.Linkstore.DependencyInjection;
using JumpsellerSync.BusinessLogic.Provider.Linkstore.Options;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Impl.DbContexts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

using System;
using System.Net.Mime;

using static JumpsellerSync.BusinessLogic.Provider.Linkstore.Constants.HttpClientConstants;

namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.Extensions
{
    public static class LinkstoreStartupExtensions
    {
        public static ContainerBuilder ConfigureLinkstoreContainer(
            this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterInstance(
                new DbContextType
                {
                    ContextType = typeof(LinkstoreNpgsqlDbContext)
                }).SingleInstance();
            builder.AddDbContext<LinkstoreNpgsqlDbContext>(configuration, "Linkstore");
            builder.RegisterModule(new RepositoriesModule(configuration));
            builder.RegisterModule(new ServicesModule());

            return builder;
        }

        public static IServiceCollection ConfigureLinkstoreServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<LinkstoreOptions>(configuration.GetSection(LinkstoreOptions.CONFIG_SECTION))
                .AddHttpClient(configuration);
        }

        public static IServiceCollection AddHttpClient(
            this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(LinkstoreOptions.CONFIG_SECTION).Get<LinkstoreOptions>();


            services.AddHttpClient(LINKSTORE_HTTP_CLIENT, client =>
            {
                client.BaseAddress = new UriBuilder(options.BaseUrl).Uri;
                client.AddBasicAuth(options.Access.Username, options.Access.Password);
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            });

            return services;
        }
    }
}
