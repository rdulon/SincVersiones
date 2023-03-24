using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Core.Dtos.Provider
{
    public class ProviderProductDetailsDto
    {
        public string Id { get; set; }

        public double Price { get; set; }

        public double Stock { get; set; }

        public ProviderBrandDetailsDto Brand { get; set; }

        public string SKU { get; set; }

        public string Description { get; set; }

        public ProviderCategoryDetailsDto Category { get; set; }

        public double Weight { get; set; }

        public List<string> ImageUrls { get; set; }
    }

    public class ReadProviderProductSuggestionsDto
    {
        public string SkuOrName { get; set; }

        public string ProviderId { get; set; }

        public string BrandId { get; set; }

        public int SuggestionsLimit { get; set; }

        public bool WithStock { get; set; }
    }

    public class SearchUnsyncedProductsDto : ReadPageDto
    {
        public string ProviderId { get; set; }

        public string BrandId { get; set; }

        public string SkuOrName { get; set; }

        public bool WithStock { get; set; }
    }

    public class SearchProviderUnsyncedProductsDto : ReadPageDto
    {
        public string BrandId { get; set; }

        public string SkuOrName { get; set; }

        public bool WithStock { get; set; }
    }

    public class ReadUnsyncedProductSuggestionsDto
    {
        public string BrandId { get; set; }

        public string SkuOrName { get; set; }

        public int SuggestionsLimit { get; set; }

        public bool WithStock { get; set; }
    }
}
