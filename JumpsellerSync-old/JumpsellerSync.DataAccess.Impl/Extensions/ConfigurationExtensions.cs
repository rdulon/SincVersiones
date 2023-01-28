using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace JumpsellerSync.DataAccess.Impl.Extensions
{
    internal static class ConfigurationExtensions
    {
        public static PropertyBuilder<ICollection<T>> HasCollectionConversion<T>(
            this PropertyBuilder<ICollection<T>> property)
        {
            if (property == null)
            { throw new ArgumentNullException(nameof(property)); }

            return property.HasConversion(
                    urls => JsonSerializer.Serialize(urls ?? new List<T>(), default),
                    str => JsonSerializer.Deserialize<ICollection<T>>(str, default));
        }

        public static void WithValueComparer<T>(
            this PropertyBuilder<ICollection<T>> property)
        {
            if (property == null)
            { throw new ArgumentNullException(nameof(property)); }

            property.Metadata.SetValueComparer(
                new ValueComparer<ICollection<T>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToHashSetCollection()));
        }

        private static ICollection<T> ToHashSetCollection<T>(this ICollection<T> collection)
        {
            return collection.ToHashSet();
        }
    }
}
