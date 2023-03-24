using JumpsellerSync.BusinessLogic.Provider.Impl.Mappings;
using JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Dtos;
using JumpsellerSync.Common.Util.Extensions;

using JumpsellerSync.Domain.Impl.Tecnoglobal;

using System;

namespace JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Mappings
{
    internal class ProductProfile : BaseProductProfile<TecnoglobalProduct>
    {
        public ProductProfile()
        {
            CreateMap<TecnoglobalProductDto, TecnoglobalProduct>()
                .MapFrom(p => p.Price, dto => Math.Round(dto.Precio * dto.DolarTg, 2))
                .MapFrom(p => p.Stock, dto => dto.StockDisp)
                .MapFrom(p => p.ProductCode, dto => dto.CodigoTg)
                .MapFrom(p => p.Id, dto => dto.CodigoTg)
                .MapFrom(p => p.Description, dto => dto.Descripcion)
                .MapFrom(p => p.Mpn, dto => dto.PnFabricante)
                .MapFrom(p => p.UpcEan, dto => dto.UpcEan13)
                .MapFrom(p => p.Brand, dto => new TecnoglobalBrand
                {
                    Id = dto.Marca.ToDbId(),
                    Description = dto.Marca
                })
                .MapFrom(p => p.Category, dto => new TecnoglobalCategory
                {
                    Id = dto.Categoria.ToDbId(),
                    Description = dto.Categoria
                })
                .MapFrom(p => p.Subcategory, dto => new TecnoglobalSubcategory
                {
                    Id = dto.SubCategoria.ToDbId(),
                    Description = dto.SubCategoria
                })
                .MapFrom(p => p.Offer, dto => dto.OfertaSiNo != 0)
                .MapFrom(p => p.CurrencyType, dto => dto.TipoMoneda)
                .MapFrom(p => p.TecnoglobalDollar, dto => dto.DolarTg)
                .MapFrom(
                    p => p.Timestamp,
                    dto => DateTime.ParseExact(dto.TimeStamp, "yyyy-MM-dd HH:mm:ss", default))
                .Ignore(p => p.RedcetusProductId)
                .Ignore(p => p.BrandId)
                .AfterMap((dto, prod) => prod.BrandId = prod.Brand.Id);
        }
    }
}
