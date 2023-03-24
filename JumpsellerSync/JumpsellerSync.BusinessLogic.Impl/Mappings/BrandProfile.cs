using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Main;

namespace JumpsellerSync.BusinessLogic.Impl.Mappings
{
    internal class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<ProviderBrandDetailsDto, Brand>()
                .MapFrom(brand => brand.Name, dto => dto.Description.Capitalize())
                .MapFrom(brand => brand.NormalizedName, dto => dto.Description.ToUpper())
                .MapFrom(brand => brand.Id, dto => dto.Description.ToDbId());

            CreateMap<Brand, PrefetchedBrandDto>();
        }
    }
}
