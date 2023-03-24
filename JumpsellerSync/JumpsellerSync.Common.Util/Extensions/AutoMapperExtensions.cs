using AutoMapper;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JumpsellerSync.Common.Util.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination, TMember>(
            this IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, TMember>> destinationMember)
        {
            mapping.ForMember(destinationMember, opts => opts.Ignore());
            return mapping;
        }

        /// <summary>
        /// Don't map those properties in <typeparamref name="TDestination"/> that 
        /// cannot be found on <typeparamref name="TSource"/>.
        /// Call this method before any other configuration as it will overwrite them.
        /// </summary>
        /// <typeparam name="TSource">Source type to map from</typeparam>
        /// <typeparam name="TDestination">Destination type to map to</typeparam>
        /// <param name="mapping">The mapping expression</param>
        /// <returns>The modified mapping expression.</returns>
        public static IMappingExpression<TSource, TDestination> IgnoreUnknownProperties
            <TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mapping)
        {
            var sourceProperties = typeof(TSource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(pi => pi.Name);
            var destProperties = typeof(TDestination)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(pi => pi.Name);

            foreach (var (destName, destPi) in destProperties)
            {
                if (sourceProperties.TryGetValue(destName, out var sourcePi) &&
                    destPi.PropertyType.IsAssignableFrom(sourcePi.PropertyType))
                { mapping.ForMember(destName, opts => opts.MapAtRuntime()); }
            }
            mapping.ForAllOtherMembers(opts => opts.Ignore());

            return mapping;
        }

        public static IMappingExpression<TSource, TDestination> MapFrom
            <TSource, TDestination, TSourceMember, TDestMember>(
            this IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, TDestMember>> destMemberSelector,
            Expression<Func<TSource, TSourceMember>> mapExpression)
                => mapping.ForMember(destMemberSelector, opts => opts.MapFrom(mapExpression));
    }
}
