using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;

using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Main
{
    public interface ISynchronizeProvidersService : IService
    {
        Task SynchronizeProvidersAsync();

        Task<ServiceResult> SynchronizeProviderAsync(string providerId);

        Task<ServiceResult> SynchronizeProviderProductsAsync(
            string syncSessionId, SynchronizationInfoDto syncInfo);
    }
}
