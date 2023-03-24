using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Core.Dtos.Provider
{
    public class SynchronizeSkuDto
    {
        public IEnumerable<SynchronizeProductSkuDto> SkuInfo { get; set; }
    }

    public class SynchronizeProductSkuDto
    {
        public string Sku { get; set; }

        public string RedcetusId { get; set; }
    }
}
