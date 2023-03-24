using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;

using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Provider
{
    public interface ISynchronizeService : IService
    {
        Task<ServiceResult> SynchronizeAsync(SynchronizationInfoDto syncInfo);

        delegate ISynchronizeService Factory(string callbackUrl, string syncSessionId);
    }
}
