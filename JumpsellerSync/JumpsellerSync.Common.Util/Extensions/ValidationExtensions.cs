using FluentValidation;

using System;
using System.Linq;

namespace JumpsellerSync.Common.Util.Extensions
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> UrlAddress<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            params string[] validSchemes)
        {
            if (validSchemes?.Length == 0)
            { validSchemes = new[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps }; }

            return ruleBuilder.Must(url =>
                Uri.TryCreate(url, UriKind.Absolute, out var validUri)
                    ? validSchemes.Contains(validUri.Scheme)
                    : false);
        }
    }
}
