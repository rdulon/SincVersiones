using JumpsellerSync.BusinessLogic.Provider.Impl.Mappings;
using JumpsellerSync.BusinessLogic.Provider.Nexsys.Dtos;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Nexsys;

namespace JumpsellerSync.BusinessLogic.Provider.Nexsys.Mappings
{
    internal class BrandProfile : BaseBrandProfile<NexsysBrand>
    {
        public BrandProfile()
        {
            CreateMap<BrandDto, NexsysBrand>()
                .MapFrom(b => b.Description, dto => dto.Manufacturer)
                .MapFrom(b => b.Id, dto => dto.NexsysId.ToDbId());
        }
    }
}
