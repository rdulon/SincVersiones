using JumpsellerSync.BusinessLogic.Core.Dtos.Main;

using System.Collections.Generic;

namespace JumpsellerSync.RestApi.FrontEnd.Models
{
    public class SearchSyncedProductsViewModel
    {
        public string SkuOrName { get; set; }

        public string BrandId { get; set; }

        public int Limit { get; set; } = 10;
    }

    public class CreateLocalProductsViewModel
    {
        public IEnumerable<CreateLocalProductDto> Products { get; set; }
    }

    public class UpdateLocalProductsViewModel
    {
        public IEnumerable<UpdateLocalProductDto> Products { get; set; }
    }
}
