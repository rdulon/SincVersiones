using Autofac;

using JumpsellerSync.BusinessLogic.Provider.Impl.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Linq;
using System.Reflection;

using RestApiCoreStartup = JumpsellerSync.RestApi.Core.Startup;


namespace JumpsellerSync.RestApi.Provider.Core
{
    public abstract class Startup : RestApiCoreStartup
    {
        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            params Assembly[] registerValidatorsMappers)
            : base(
                  configuration, environment,
                  registerValidatorsMappers?.Concat(
                      new[] { Assembly.GetExecutingAssembly() }).ToArray() ??
                  new[] { Assembly.GetExecutingAssembly() })
        { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.ConfigureBusinessLogicProviderServices(Configuration);
        }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            builder.ConfigureBusinessLogicProviderContainer(Configuration);
        }
    }
}
