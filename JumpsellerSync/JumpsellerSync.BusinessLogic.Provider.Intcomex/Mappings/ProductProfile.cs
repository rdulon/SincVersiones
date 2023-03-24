using JumpsellerSync.BusinessLogic.Provider.Impl.Mappings;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Dtos;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Intcomex;

namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.Mappings
{
    internal class ProductProfile : BaseProductProfile<IntcomexProduct>
    {
        public ProductProfile()
        {
            CreateMap<IntcomexProductDto, IntcomexProduct>()
                .MapFrom(p => p.Id, dto => dto.Mpn)
                .MapFrom(p => p.ProductCode, dto => dto.Mpn)
                .MapFrom(p => p.Stock, dto => dto.InStock)
                .MapFrom(p => p.BrandId, dto => dto.Brand != null ? dto.Brand.BrandId : null)
                .MapFrom(p => p.ProductType, dto => Convert(dto.Type))
                .MapFrom(p => p.IntcomexSku, dto => dto.Sku)
                .Ignore(p => p.Price)
                .Ignore(p => p.RedcetusProductId)
                .Ignore(p => p.Category)
                .Ignore(p => p.Dirty);
        }

        private static IntcomexProductType Convert(string type)
            => type switch
            {
                "Downloadable" => IntcomexProductType.Downloadable,
                "Warranty" => IntcomexProductType.Warranty,
                "License" => IntcomexProductType.License,
                "Physical" => IntcomexProductType.Physical,
                _ => IntcomexProductType.Unknown
            };
    }
}
