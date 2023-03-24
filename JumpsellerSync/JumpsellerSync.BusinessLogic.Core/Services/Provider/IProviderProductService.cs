using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Provider
{
    public interface IProviderProductService : IService
    {
        Task<ServiceResult<ProviderBrandDetailsDto>> ReadBrandAsync(string brandId);

        Task<IEnumerable<ProviderBrandDetailsDto>> ReadBrandsAsync(int page, int limit);

        Task<ServiceResult<ProviderProductDetailsDto>> ReadProductAsync(string productId);

        Task<PageResultDto<ProviderProductDetailsDto>> ReadUnsyncedProductsAsync(
            SearchProviderUnsyncedProductsDto search);

        Task<ServiceResult<ProviderCategoryDetailsDto>> ReadCategoryAsync(string categoryId);

        Task<IEnumerable<ProviderCategoryDetailsDto>> ReadCategoriesAsync(int page, int limit);

        Task<ServiceResult> SynchronizeProductAsync(string productId, string redcetusId);

        Task<ServiceResult> UnsynchronizeProductAsync(string redcetusId);

        Task<ServiceResult<IEnumerable<ProviderBrandInfo>>> SynchronizeProductsBySkuAsync(
            SynchronizeSkuDto info);

        Task<ServiceResult<IEnumerable<ProductSuggestionDto>>> ReadUnsyncedProductSuggestionsAsync(
            ReadUnsyncedProductSuggestionsDto input);
    }
}
