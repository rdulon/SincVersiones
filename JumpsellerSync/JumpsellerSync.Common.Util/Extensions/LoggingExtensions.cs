using Microsoft.Extensions.Logging;

using System;

namespace JumpsellerSync.Common.Util.Extensions
{
    public static class LoggingExtensions
    {
        public static bool LogException<T>(this ILogger<T> logger, Exception e, string header = default)
        {
            logger.LogError($"{header ?? "An error has occurred"}: {e?.Message}.\n\n" +
                $"Stack trace: {e?.StackTrace}");

            return false;
        }
    }
}
