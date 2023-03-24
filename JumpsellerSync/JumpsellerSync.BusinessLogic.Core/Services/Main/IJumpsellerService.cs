
using JumpsellerSync.BusinessLogic.Core.Dtos.Jumpseller;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Main
{
    public interface IJumpsellerService : IService
    {
        ServiceResult<string> GetAuthorizeUrlAsync(int? storeId);

        Task SynchronizeProductsAsync();

        Task SynchronizeLocalProductsAsync();

        Task SynchronizeLocalProductsAsync(IEnumerable<string> localProductIds);

        Task<ServiceResult> ExchangeCodeAsync(string code);

        Task<ServiceResult> SynchronizeProductAsync(string productId);

        Task<ServiceResult<JumpsellerProductWrapperDto>> ReadProductAsync(
            int jumpsellerProductId);

        Task<ServiceResult<JumpsellerProductWrapperDto>> ReadProductBySkuAsync(string sku);

        Task<ServiceResult> DeleteProductAsync(int jumpsellerProductId);

        Task<ServiceResult<LoadProductsResultDto>> LoadProductsAsync();
    }
}
