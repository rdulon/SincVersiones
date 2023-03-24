using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Main;

namespace JumpsellerSync.BusinessLogic.Impl.Mappings
{
    internal class LocalProductProfile : Profile
    {
        public LocalProductProfile()
        {
            CreateMap<Product, LocalProduct>()
                .Ignore(local => local.Id)
                .MapFrom(local => local.Product, product => product)
                .MapFrom(local => local.ProductId, product => product.Id)
                .MapFrom(local => local.Brand, product => product.Brand)
                .MapFrom(local => local.BrandId, product => product.Brand.Id);

            CreateMap<LocalProduct, LocalProductDetailsDto>()
                .MapFrom(dto => dto.Sku, local => local.SKU);

            CreateMap<UpdateLocalProductDto, LocalProduct>()
                .IgnoreUnknownProperties()
                .ForMember(p => p.Stock, opts => opts.MapAtRuntime());
        }
    }
}
