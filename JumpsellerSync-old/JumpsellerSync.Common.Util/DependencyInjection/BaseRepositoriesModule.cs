using Autofac;

using Microsoft.Extensions.Configuration;

using System;
using System.Reflection;
using System.Text.RegularExpressions;

using static JumpsellerSync.Common.Util.Constants.ConfigurationConstants;

namespace JumpsellerSync.Common.Util.DependencyInjection
{
    public abstract class BaseRepositoriesModule : Autofac.Module
    {
        private readonly Regex REPO_NAME_RE;
        private readonly TypeInfo assemblyMarkerInfo;

        public BaseRepositoriesModule(
            IConfiguration configuration,
            Type assemblyMarker,
            params string[] additionalLevels)
        {
            assemblyMarkerInfo = assemblyMarker.GetTypeInfo();
            var baseNamespace = assemblyMarker.Namespace;
            var providerName = configuration.GetValue<string>(PROVIDER_NAME_KEY);

            var levels = string.Join('.', additionalLevels ?? new string[0]);
            if (!string.IsNullOrEmpty(levels))
            { levels = $".{levels}"; }

            REPO_NAME_RE = new Regex(
                @$"^{Regex.Escape(baseNamespace)}\." +
                @$"{providerName}{Regex.Escape(levels)}\..+?Repository$");
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(assemblyMarkerInfo.Assembly)
                .PublicOnly()
                .Where(t => !t.IsAbstract && t.IsClass && REPO_NAME_RE.IsMatch(t.FullName))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
