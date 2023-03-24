namespace JumpsellerSync.RestApi.FrontEnd.Models
{
    public class ReadProductSuggestionsViewModel
    {
        public bool SyncedProducts { get; set; }

        public string SkuOrName { get; set; }

        public string ProviderId { get; set; }

        public string BrandId { get; set; }

        public int SuggestionsLimit { get; set; } = 10;

        public bool WithStock { get; set; }
    }
}
