using Autofac;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JumpsellerSync.Common.Util.DependencyInjection
{
    public abstract class BaseServicesModule : Autofac.Module
    {
        private readonly Regex SERVICE_NAME_RE;
        private readonly TypeInfo assemblyMarkerInfo;
        private readonly HashSet<Type> autoWireProperties;

        public BaseServicesModule(Type assemblyMarker, params Type[] autoWireProperties)
        {
            assemblyMarkerInfo = assemblyMarker.GetTypeInfo();
            this.autoWireProperties = new HashSet<Type>(autoWireProperties ?? new Type[0]);
            var @namespace = assemblyMarkerInfo.Namespace;
            SERVICE_NAME_RE = new Regex(@$"^{Regex.Escape(@namespace)}\..+?Service$");
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(assemblyMarkerInfo.Assembly)
                .PublicOnly()
                .Where(t => !autoWireProperties.Contains(t) &&
                    !t.IsAbstract && t.IsClass &&
                    SERVICE_NAME_RE.IsMatch(t.FullName))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            foreach (var t in autoWireProperties)
            {
                builder
                    .RegisterType(t)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            }
        }
    }
}
