using JumpsellerSync.BusinessLogic.Provider.Impl.Mappings;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Dtos;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Intcomex;

namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.Mappings
{
    internal class BrandProfile : BaseBrandProfile<IntcomexBrand>
    {
        public BrandProfile()
        {
            CreateMap<IntcomexBrandDto, IntcomexBrand>()
                .MapFrom(b => b.Id, dto => dto.BrandId);
        }
    }
}
