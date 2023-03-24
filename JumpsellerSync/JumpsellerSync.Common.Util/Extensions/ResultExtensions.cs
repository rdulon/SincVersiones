namespace JumpsellerSync.Common.Util.Extensions
{
    public static class ResultExtensions
    {
        public static string[] ToFailureErrors(
            this string[] src, string defaultMessage = "Error desconocido.")
                => src == null || src.Length == 0
                    ? new[] { defaultMessage }
                    : src;
    }
}
