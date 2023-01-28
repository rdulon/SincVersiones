using Microsoft.Extensions.Configuration;

namespace JumpsellerSync.Common.Util.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetConnectionString(
            this IConfiguration configuration, string connectionName, string providerName)
        {
            try
            {
                return configuration.GetValue<string>(
                    $"ConnectionStrings:{providerName}:{connectionName}:ConnectionString");
            }
            catch { return default; }
        }

        public static string GetPassword(
            this IConfiguration configuration, string connectionName, string providerName)
        {
            try
            {
                return configuration.GetValue<string>(
                    $"ConnectionStrings:{providerName}:{connectionName}:Password");
            }
            catch { return default; }

        }

        public static string GetSchema(
            this IConfiguration configuration, string connectionName, string providerName)
        {
            try
            {
                return configuration.GetValue<string>(
                    $"ConnectionStrings:{providerName}:{connectionName}:Schema");
            }
            catch { return default; }
        }
    }
}
