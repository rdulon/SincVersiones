using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Main
{
    public interface IProviderHelperService : IService
    {
        Task<bool> IsProviderReachableAsync(string providerUrl);

        string WrapProviderSessionId(string sessionId, string providerId);

        (string, string) UnwrapProviderSessionId(string providerSessionId);
    }
}
