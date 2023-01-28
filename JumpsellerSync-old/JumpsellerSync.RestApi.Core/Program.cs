using Autofac.Extensions.DependencyInjection;

using JumpsellerSync.DataAccess.Core;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Net;
using System.Threading.Tasks;
namespace JumpsellerSync.RestApi.Core
{
    public abstract class Program<TStartup>
        where TStartup : class
    {
        public static int Main(
            string[] args, int? kestrelPort = default)
        {
            var cmdApp = new CommandLineApplication(false);
            var applyEFMigrations = cmdApp.Option(
                "--apply-migrations",
                "Apply Entity Framework Core migrations and exit.",
                CommandOptionType.NoValue);
            cmdApp.HelpOption("-h | --help");
            cmdApp.OnExecute(async () =>
            {
                await RunApp(args, kestrelPort, applyEFMigrations);
                return 0;
            });
            return cmdApp.Execute(args);
        }

        private static async Task RunApp(
            string[] args, int? kestrelPort, CommandOption applyEFMigrations)
        {
            var host = CreateHostBuilder(args, kestrelPort)
                       .Build();

            if (applyEFMigrations.HasValue())
            {
                var dbContextType = host.Services.GetService<DbContextType>();
                using var context = (DbContext)host.Services.GetService(
                    dbContextType.ContextType);
                context.Database.Migrate();
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(
            string[] args, int? kestrelPort = default)
                => Host.CreateDefaultBuilder(args)
                   .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.UseStartup<TStartup>();
                       var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                       if (kestrelPort != null && env != Environments.Development)
                       {
                           webBuilder.UseKestrel(kestrelOpts =>
                           {
                               kestrelOpts.Listen(IPAddress.Loopback, kestrelPort.Value);
                           });
                       }
                   });

    }
}
