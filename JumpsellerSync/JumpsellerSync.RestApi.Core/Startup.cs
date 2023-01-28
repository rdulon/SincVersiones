using Autofac;
using Autofac.Extensions.DependencyInjection;

using FluentValidation.AspNetCore;

using Hangfire;

using JumpsellerSync.Common.Util.DependencyInjection;
using JumpsellerSync.Common.Util.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Reflection;
using System.Text.Json;

namespace JumpsellerSync.RestApi.Core
{
    public abstract class Startup
    {
        private readonly Assembly[] registerValidatorsMappers;

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            params Assembly[] registerValidatorsMappers)
        {
            Configuration = configuration;
            Environment = environment;

            registerValidatorsMappers ??= new Assembly[0];
            this.registerValidatorsMappers = new Assembly[registerValidatorsMappers.Length + 1];
            Array.Copy(
                registerValidatorsMappers, 0, this.registerValidatorsMappers,
                0, registerValidatorsMappers.Length);
            this.registerValidatorsMappers[^1] = Assembly.GetExecutingAssembly();
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; set; }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {

            builder.RegisterModule(new AutoMapperModule(registerValidatorsMappers));
            builder.RegisterInstance(new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = false,
                ReadCommentHandling = JsonCommentHandling.Skip
            }).SingleInstance();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.ConfigureHangfire(Configuration);
            services.AddMvc().AddFluentValidation(
                config => config.RegisterValidatorsFromAssemblies(registerValidatorsMappers));
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            GlobalConfiguration.Configuration.UseAutofacActivator(app.ApplicationServices.GetAutofacRoot(), false);

            if (Environment.IsDevelopment())
            { app.UseDeveloperExceptionPage(); }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
