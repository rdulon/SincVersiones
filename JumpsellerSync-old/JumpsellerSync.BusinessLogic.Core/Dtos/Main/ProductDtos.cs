using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Core.Dtos.Main
{
    public class CreateProductDto
    {
        public string ProviderId { get; set; }

        public string ProviderProductId { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public int Margin { get; set; }
    }

    public class ProductDetailsDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public double Stock { get; set; }

        public string SKU { get; set; }

        public string Brand { get; set; }

        public double Weight { get; set; }

        public int Margin { get; set; }
    }

    public class ReadProductSuggestionsDto
    {
        public string SkuOrName { get; set; }

        public int SuggestionsLimit { get; set; }
    }

    public class SearhProductsDto : ReadPageDto
    {
        public string SkuOrName { get; set; }

        public IEnumerable<string> BrandIds { get; set; }
    }
}
