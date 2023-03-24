using Autofac;

using JumpsellerSync.BusinessLogic.Impl.Extensions;
using JumpsellerSync.BusinessLogic.Impl.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

using CoreStartup = JumpsellerSync.RestApi.Core.Startup;

namespace JumpsellerSync.Frontend.RestApi
{
    public sealed class Startup : CoreStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
            : base(configuration, environment,
                   Assembly.GetExecutingAssembly(),
                   typeof(BaseService<,,,,>).Assembly)
        { }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            builder.ConfigureBusinessLogicContainer(Configuration);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.ConfigureBusinessLogicServices(Configuration);
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto
            });

            if (!Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseEndpoints(config =>
            {
                config.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.ConfigureBusinessLogicApp();
        }
    }
}
