using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;

using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Provider
{
    public interface IProviderService : IService
    {
        Task SynchronizeProductsAsync(StartSynchronizationDto syncInfo);

        Task<ServiceResult<ProviderProductDetailsDto>> SynchronizeWithRedcetusAsync(
            string productId, string redcetusProductId);
    }
}
