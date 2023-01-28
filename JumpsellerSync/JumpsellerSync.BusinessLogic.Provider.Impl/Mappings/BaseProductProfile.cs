using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Provider;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Mappings
{
    public abstract class BaseProductProfile<TProduct> : Profile
        where TProduct : ProviderProduct
    {
        public BaseProductProfile()
        {
            CreateMap<TProduct, ProviderProductDetailsDto>()
               .MapFrom(dto => dto.SKU, p => p.ProductCode)
               .Ignore(dto => dto.Weight)
               .Ignore(dto => dto.ImageUrls);

            CreateMap<TProduct, SynchronizeProductDto>()
                .MapFrom(dto => dto.ProductId, p => p.RedcetusProductId);
        }
    }
}
