using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.QueryModels.Provider;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProviderUnsyncedProductSuggestion, ProductSuggestionDto>()
                .MapFrom(dto => dto.Value, sugg => sugg.ProductDescription)
                .MapFrom(dto => dto.Id, sugg => sugg.ProductId);

        }
    }
}
