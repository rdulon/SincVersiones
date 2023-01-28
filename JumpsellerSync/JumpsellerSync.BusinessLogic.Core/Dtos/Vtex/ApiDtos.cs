using JumpsellerSync.BusinessLogic.Core.Dtos.Jumpseller;
using System.Collections.Generic;
using System.Net.Http;

namespace JumpsellerSync.BusinessLogic.Core.Dtos.Vtex
{
    public class VtexProductWrapperDto
    {
        public ProductDto Product { get; set; }
    }

    public class ProductToSyncDto
    {
        public virtual int? JumpsellerId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual double Price { get; set; }

        public virtual int Margin { get; set; }

        public virtual double Stock { get; set; }

        public virtual string SKU { get; set; }

        public virtual string BrandId { get; set; }
        public virtual BrandDto Brand { get; set; }

        public virtual ICollection<string> ImageUrls { get; set; } = new HashSet<string>();

        public virtual double Weight { get; set; }

        public virtual double Width { get; set; }

        public virtual double Height { get; set; }

        public virtual double Length { get; set; }

        public virtual bool IsDigital { get; set; }

        public virtual bool SynchronizedToJumpseller { get; set; }

        public virtual ICollection<string> SynchronizingProviderIds { get; set; } = new List<string>();

        public bool IsLocalProduct { get; set; }
    }

    public class ProductDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string DepartmentId { get; set; }

        public string BrandId { get; set; }

        public string Description { get; set; }

        public string CategoryId { get; set; }

        public string RefId { get; set; }

        public bool IsVisible { get; set; }

        public bool IsActive { get; set; }

        public string Title { get; set; }

        public bool ActivateIfPossible { get; set; }

        public bool ShowWithoutStock { get; set; }

        public string LinkId { get; set; }
    }

    public class BrandDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }

    public class SkuDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string RefId { get; set; }

        public double PackagedHeight { get; set; }

        public double PackagedLength { get; set; }

        public double PackagedWidth { get; set; }

        public double PackagedWeightKg { get; set; }

        public string ManufacturerCode { get; set; }

        public bool ActivateIfPossible { get; set; }

        public string UnitMultiplier { get; set; }

        public string MeasurementUnit { get; set; }
    }

    public class SkuImageDto
    {
        public bool IsMain { get; set; }

        public string Label { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }

    public class SkuPriceDto
    {
        public double markup { get; set; }

        public double listPrice { get; set; }

        public double basePrice { get; set; }
    }

    public class SkuStockDto
    {
        public bool unlimitedQuantity { get; set; }

        public string dateUtcOnBalanceSystem { get; set; }

        public int quantity { get; set; }
    }

    public class JumpsellerProductsCountDto
    {
        public int Count { get; set; }
    }

    public class VtexResponse
    {
        public int? id;

        public HttpResponseMessage response;
    }
}
