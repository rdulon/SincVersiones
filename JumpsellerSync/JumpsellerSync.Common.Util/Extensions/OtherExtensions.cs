using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace JumpsellerSync.Common.Util.Extensions
{
    public static class OtherExtensions
    {
        public static int GetDbOffset(this int page, int limit)
            => Math.Max((page - 1) * limit, 0);

        public static string AsLogHeader(this string prefix, char separator = '.')
        {
            var guid = Guid.NewGuid().ToString("N");

            if (string.IsNullOrEmpty(prefix))
            { return $"[{guid}]"; }

            prefix += separator;

            return $"[{prefix}{guid}]";
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection != null)
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
        }

        public static string Capitalize(this string str)
        {
            return string.IsNullOrEmpty(str)
                ? str
                : str[0].ToString().ToUpper() + str.Substring(1).ToLower();
        }

        public static Uri Augment(this Uri baseUri, string path, NameValueCollection query = default)
        {
            var havePath = !string.IsNullOrEmpty(path);
            var uriBuilder = new UriBuilder(baseUri);
            if (havePath)
            { uriBuilder.Path = $"{uriBuilder.Path.TrimEnd('/')}/{path.TrimStart('/')}"; }

            query ??= new NameValueCollection();
            var q = string.Join(
                '&',
                query.AllKeys
                     .SelectMany(
                        k => query.GetValues(k).Select(v => $"{k}={Uri.EscapeDataString(v)}")));
            if (!string.IsNullOrEmpty(q))
            {
                if (!string.IsNullOrEmpty(uriBuilder.Query))
                { uriBuilder.Query += "&"; }
                uriBuilder.Query += q;
            }

            return uriBuilder.Uri;
        }

        public static string ToDbId(this string str)
        {
            if (string.IsNullOrEmpty(str))
            { return null; }

            var cleanStr = Regex.Replace(str, @"\s+", "_").ToLower();
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(cleanStr));
            var idBuilder = new StringBuilder();
            foreach (var @byte in bytes)
            {
                idBuilder.Append(@byte.ToString("x2"));
            }
            return idBuilder.ToString();
        }
    }
}
