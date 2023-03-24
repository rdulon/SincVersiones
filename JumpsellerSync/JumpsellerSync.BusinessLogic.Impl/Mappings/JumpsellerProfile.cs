using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Jumpseller;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Main;

using System;

namespace JumpsellerSync.BusinessLogic.Impl.Mappings
{
    internal class JumpsellerProfile : Profile
    {
        public JumpsellerProfile()
        {
            CreateMap<AuthTokensDto, JumpsellerConfiguration>()
                .MapFrom(config => config.AccessTokenType, dto => dto.TokenType)
                .MapFrom(
                    config => config.TokenCreatedAt,
                    dto => DateTimeOffset.FromUnixTimeSeconds(dto.CreatedAt).DateTime)
                .Ignore(config => config.ApplicationAuthorized)
                .AfterMap((dto, config) =>
                {
                    config.TokenExpiresAt
                        = config.TokenCreatedAt.Value.AddSeconds(dto.ExpiresIn);
                });

            CreateMap<Product, JumpsellerProductDto>()
                    .MapFrom(dto => dto.Id, p => p.JumpsellerId)
                    .MapFrom(
                        dto => dto.Price,
                        p => Math.Round(p.Price * 119 * (p.Margin + 100) / 10000D))
                    .MapFrom(dto => dto.StockUnlimited, p => false)
                    .MapFrom(dto => dto.Sku, p => p.SKU)
                    .MapFrom(dto => dto.ShippingRequired, p => !p.IsDigital)
                    .MapFrom(
                        dto => dto.Status,
                        p => p.Stock > 0 ? "available" : "not-available")
                    .MapFrom(
                        dto => dto.PackageFormat,
                        p => p.Format.ToString("g").ToLower())
                    .MapFrom(dto => dto.Brand, product => product.Brand.Name)
                .ReverseMap()
                    .IgnoreUnknownProperties()
                    .MapFrom(p => p.JumpsellerId, dto => dto.Id)
                    .MapFrom(p => p.SynchronizedToJumpseller, _ => true)
                    .MapFrom(p => p.Margin, _ => 20)
                    .MapFrom(p => p.Brand, dto => new Brand
                    {
                        Id = GetBrand(dto).ToDbId(),
                        Name = GetBrand(dto),
                        NormalizedName = GetBrand(dto).ToUpper()
                    })
                    .AfterMap((_, p) => p.BrandId = p.Brand.Id);

            var productConverter = new ProductJumpsellerWrapperProductTypeConverter();
            CreateMap<Product, JumpsellerProductWrapperDto>()
                .ConvertUsing(productConverter);
            CreateMap<JumpsellerProductWrapperDto, Product>()
                .ConvertUsing(productConverter);
        }

        private static string GetBrand(JumpsellerProductDto dto)
            => string.IsNullOrEmpty(dto.Brand) ? "Otras Marcas" : dto.Brand;
    }

    internal class ProductJumpsellerWrapperProductTypeConverter
        : ITypeConverter<Product, JumpsellerProductWrapperDto>,
          ITypeConverter<JumpsellerProductWrapperDto, Product>
    {
        public JumpsellerProductWrapperDto Convert(
            Product source, JumpsellerProductWrapperDto destination, ResolutionContext context)
        {
            destination ??= new JumpsellerProductWrapperDto();
            destination.Product ??= new JumpsellerProductDto();
            context.Mapper.Map(
                source, destination.Product, typeof(Product), typeof(JumpsellerProductDto));
            return destination;
        }

        public Product Convert(
            JumpsellerProductWrapperDto source, Product destination, ResolutionContext context)
        {
            destination ??= new Product();
            context.Mapper.Map(
                source.Product, destination, typeof(JumpsellerProductDto), typeof(Product));
            return destination;
        }
    }
}
