using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.RestApi.Provider.Core.Models;
namespace JumpsellerSync.RestApi.Provider.Core.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<LoadProductsPageViewModel, SearchProviderUnsyncedProductsDto>()
                .Ignore(dto => dto.BrandId);
        }
    }
}
