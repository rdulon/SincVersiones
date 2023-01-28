using Autofac;

using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Extensions;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

using ProviderStartupCore = JumpsellerSync.RestApi.Provider.Core.Startup;

namespace JumpsellerSync.RestApi.Provider.Linkstore
{
    public sealed class Startup : ProviderStartupCore
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
            : base(configuration, environment,
                  Assembly.GetExecutingAssembly(),
                  typeof(IntcomexProviderService).Assembly,
                  typeof(ProviderService<,,,>).Assembly)
        { }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            builder.ConfigureIntcomexContainer(Configuration);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.ConfigureIntcomexServices(Configuration, Environment);
        }
    }
}
