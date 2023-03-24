using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.Domain.Impl.Provider;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Mappings
{
    public abstract class BaseCategoryProfile<TCategory> : Profile
        where TCategory : ProviderCategory
    {
        public BaseCategoryProfile()
        {
            CreateMap<TCategory, ProviderCategoryDetailsDto>();
        }
    }
}
