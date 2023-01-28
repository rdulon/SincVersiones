
using JumpsellerSync.BusinessLogic.Core.Dtos.Vtex;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace JumpsellerSync.BusinessLogic.Core.Services.Main
{
    public interface IVtexService : IService
    {
        Task SynchronizeProductsAsync();
        Task SynchronizeLocalProductsAsync();

        Task SynchronizeLocalProductsAsync(IEnumerable<string> localProductIds);

        Task<ServiceResult> SynchronizeProductAsync(string productId);

        Task<JsonElement?> ReadProductByRefAsync(string sku);

        Task<ServiceResult<LoadProductsResultDto>> LoadProductsAsync();
    }
}
