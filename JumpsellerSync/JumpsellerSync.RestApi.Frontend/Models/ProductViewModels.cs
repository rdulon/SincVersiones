using JumpsellerSync.BusinessLogic.Core.Dtos;

namespace JumpsellerSync.RestApi.FrontEnd.Models
{
    public class LoadUnsyncedProductsPageViewModel : ReadPageDto
    {
        public string ProviderId { get; set; }

        public string BrandId { get; set; }

        public string SkuOrName { get; set; }

        public bool WithStock { get; set; }
    }

    public class LoadSyncedProductsPageViewModel : ReadPageDto
    {
        public string SkuOrName { get; set; }

        public string BrandIds { get; set; }
        public string Provider { get; set; }
    }
}
