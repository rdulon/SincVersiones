using JumpsellerSync.BusinessLogic.Core.Dtos.Main;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Main
{
    public interface ILocalProductService
        : IReadService<LocalProductDetailsDto>,
          IDeleteService
    {
        Task<ServiceResult> CreateAsync(IEnumerable<CreateLocalProductDto> localProducts);

        Task<ServiceResult> UpdateAsync(IEnumerable<UpdateLocalProductDto> localProducts);

        Task<IEnumerable<ProductDetailsDto>> SearchSyncedProductsAsync(
            string brandId, string skuOrName, int limit);
    }
}
