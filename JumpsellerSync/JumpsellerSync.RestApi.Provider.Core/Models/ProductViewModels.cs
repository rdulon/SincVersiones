using JumpsellerSync.BusinessLogic.Core.Dtos;

namespace JumpsellerSync.RestApi.Provider.Core.Models
{
    public class LoadProductsPageViewModel : ReadPageDto
    {
        public string SkuOrName { get; set; }

        public bool WithStock { get; set; }
    }
}
