using Autofac;

using Hangfire;
using Hangfire.PostgreSql;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Npgsql;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using static JumpsellerSync.Common.Util.Constants.ConfigurationConstants;
using static JumpsellerSync.Common.Util.Constants.DataAccessConstants;

namespace JumpsellerSync.Common.Util.Extensions
{
    public static class StartupExtensions
    {
        public static ContainerBuilder AddDbContext<TDbContext>(
            this ContainerBuilder builder, IConfiguration configuration, string connName)
            where TDbContext : DbContext
        {
            var provider = configuration.GetValue<string>(PROVIDER_NAME_KEY, default);
            switch (provider)
            {
                case NPGSQL_PROVIDER_NAME:
                    builder
                        .Register(
                            ctx =>
                            {
                                var conn = configuration.GetConnectionString(connName, NPGSQL_PROVIDER_NAME);
                                var dbConnection = new NpgsqlConnection(conn);
                                var password = configuration.GetPassword(connName, NPGSQL_PROVIDER_NAME);
                                dbConnection.ProvidePasswordCallback =
                                    (host, port, database, username) => password;

                                var dbContextOptions = new DbContextOptions<TDbContext>(
                                    new Dictionary<Type, IDbContextOptionsExtension>());
                                var optionsBuilder = new DbContextOptionsBuilder<TDbContext>(dbContextOptions);
                                var provider = ctx.Resolve<IServiceProvider>();

                                optionsBuilder
                                    .UseLazyLoadingProxies()
                                    .UseApplicationServiceProvider(provider)
                                    .UseNpgsql(dbConnection);
                                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                                if (env == Environments.Development)
                                {
                                    Console.Out.WriteLine(
                                        "**\nDatabase sensitive data logging enabled\n**");
                                    optionsBuilder.EnableSensitiveDataLogging();
                                }

                                return optionsBuilder.Options;
                            })
                        .As<DbContextOptions<TDbContext>>()
                        .InstancePerLifetimeScope()
                        ;
                    builder
                        .Register(ctx => ctx.Resolve<DbContextOptions<TDbContext>>())
                        .As<DbContextOptions>()
                        .InstancePerLifetimeScope();
                    builder
                        .RegisterType<TDbContext>()
                        .AsSelf()
                        .InstancePerLifetimeScope();
                    break;
            }

            return builder;
        }

        public static IServiceCollection ConfigureHangfire(
           this IServiceCollection services, IConfiguration configuration)
        {
            var dbProvider = configuration.GetValue<string>(PROVIDER_NAME_KEY);

            return dbProvider switch
            {
                NPGSQL_PROVIDER_NAME => services.AddHangfire(
                    config =>
                    {
                        var conn = configuration.GetConnectionString("Hangfire", NPGSQL_PROVIDER_NAME);
                        var password = configuration.GetPassword("Hangfire", NPGSQL_PROVIDER_NAME);

                        var pgsqlOptions = new PostgreSqlStorageOptions();
                        var schemaName = configuration.GetSchema("Hangfire", NPGSQL_PROVIDER_NAME);
                        if (!string.IsNullOrEmpty(schemaName))
                        { pgsqlOptions.SchemaName = schemaName; }

                        config
                            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                            .UseSimpleAssemblyNameTypeSerializer()
                            .UseRecommendedSerializerSettings()
                            .UsePostgreSqlStorage(
                                conn,
                                dbConn =>
                                {
                                    dbConn.ProvidePasswordCallback =
                                      (host, port, database, username) => password;
                                }, pgsqlOptions);

                    }),
                _ => services
            };
        }

        public static HttpClient AddBasicAuth(this HttpClient client, string username, string password)
        {
            var digest = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{username}:{password}"));
            var auth = new AuthenticationHeaderValue("Basic", digest);
            client.DefaultRequestHeaders.Authorization = auth;

            return client;
        }
    }
}
