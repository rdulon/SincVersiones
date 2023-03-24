
using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Main
{
    public interface IProductService
        : ICreateService<CreateProductDto, ProductDetailsDto>,
          IReadService<ProductDetailsDto>,
          IDeleteService
    {
        Task<ServiceResult> UpdateMarginAsync(ProductDetailsDto product);

        Task<ServiceResult<IEnumerable<PrefetchedBrandDto>>> PrefetchBrandsAsync();

        Task<IEnumerable<ProductSuggestionDto>> ReadProductSuggestionsAsync(
            ReadProductSuggestionsDto input);

        Task<PageResultDto<ProductDetailsDto>> ReadAsync(SearhProductsDto search);
    }
}
