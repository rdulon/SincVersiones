using System;
using System.Collections.Generic;

namespace JumpsellerSync.Common.Util.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> ToBatchs<T>(
            this IEnumerable<T> source, int batchSize)
        {
            if (source == null)
            { yield break; }
            using var ie = source.GetEnumerator();
            while (ie.MoveNext())
            { yield return ie.YieldBatch(batchSize - 1); }
        }

        private static IEnumerable<T> YieldBatch<T>(this IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (var i = 0; i < batchSize && source.MoveNext(); i++)
            { yield return source.Current; }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            { yield break; }

            foreach (var item in source)
            {
                action?.Invoke(item);
                yield return item;
            }
        }
    }
}
