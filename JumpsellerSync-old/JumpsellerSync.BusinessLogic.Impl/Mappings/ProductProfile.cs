using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Main;

namespace JumpsellerSync.BusinessLogic.Impl.Mappings
{
    internal class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<SynchronizeProductDto, SynchronizationSessionInfo>();

            CreateMap<ProviderProductDetailsDto, Product>()
                .IgnoreUnknownProperties()
                .ForMember(product => product.Brand, opts => opts.MapAtRuntime())
                .Ignore(product => product.Id)
                .MapFrom(product => product.Format, dto => ProductFormat.Box)
                .MapFrom(
                    product => product.Name,
                    dto => $"{dto.Brand.Description.Capitalize()} {dto.SKU}")
                .AfterMap((_, p) => p.BrandId = p.Brand.Id);

            CreateMap<Product, ProductDetailsDto>();

            CreateMap<Product, ProductSuggestionDto>()
                .MapFrom(dto => dto.Value, product => product.Name);
        }
    }
}
