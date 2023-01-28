using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.Domain.Impl.Provider;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Mappings
{
    public abstract class BaseBrandProfile<TBrand> : Profile
        where TBrand : ProviderBrand
    {
        public BaseBrandProfile()
        {
            CreateMap<TBrand, ProviderBrandDetailsDto>();
        }
    }
}
