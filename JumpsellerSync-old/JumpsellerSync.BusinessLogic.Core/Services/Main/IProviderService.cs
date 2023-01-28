using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Main
{
    public interface IProviderService
        : IBaseService<ProviderDto, ProviderDto, ProviderDto>
    {
        Task<IEnumerable<ProviderDto>> ReadProvidersAsync();

        Task<IEnumerable<ProviderDto>> ReadActiveProvidersAsync();

        Task<IEnumerable<ProviderBrandDetailsDto>> ReadProviderBrandsAsync(string providerId);

        Task<ServiceResult<ProviderProductDetailsDto>> ReadProductAsync(
            string providerId, string providerProductId);

        Task<PageResultDto<ProviderProductDetailsDto>> ReadProductsAsync(SearchUnsyncedProductsDto search);

        Task<ServiceResult> SynchronizeProductAsync(
            string providerId, string providerProductId, string productId);

        Task<ServiceResult> UnsynchronizeProductAsync(string productId);

        Task<ServiceResult> SynchronizeProductsBySkuAsync(IEnumerable<SynchronizeProductSkuDto> skuInfo);

        Task<IEnumerable<ProductSuggestionDto>> ReadProductSuggestionsAsync(
            ReadProviderProductSuggestionsDto input);
    }
}
