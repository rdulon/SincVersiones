using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.RestApi.FrontEnd.Models;

using System;
using System.Linq;

namespace JumpsellerSync.RestApi.FrontEnd.Mappings
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<ReadProductSuggestionsViewModel, ReadProductSuggestionsDto>();
            CreateMap<ReadProductSuggestionsViewModel, ReadProviderProductSuggestionsDto>();

            CreateMap<LoadSyncedProductsPageViewModel, SearhProductsDto>()
                .MapFrom(
                    dto => dto.BrandIds,
                    vm => !string.IsNullOrEmpty(vm.BrandIds)
                            ? vm.BrandIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            : Enumerable.Empty<string>());
            CreateMap<LoadUnsyncedProductsPageViewModel, SearchUnsyncedProductsDto>();
        }
    }
}
