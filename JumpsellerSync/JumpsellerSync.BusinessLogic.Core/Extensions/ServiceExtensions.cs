namespace JumpsellerSync.BusinessLogic.Core.Extensions
{
    public static class ServiceExtensions
    {
        public static bool IsSucceed(this ServiceResult serviceResult)
            => serviceResult?.Status == ServiceResultStatus.Succeed;

        public static bool IsSucceed<TData>(this ServiceResult<TData> serviceResult)
            => serviceResult?.Status == ServiceResultStatus.Succeed;

        public static string YesNo(this ServiceResult serviceResult)
            => serviceResult != null && serviceResult.IsSucceed() ? "Yes" : "No";
    }
}
