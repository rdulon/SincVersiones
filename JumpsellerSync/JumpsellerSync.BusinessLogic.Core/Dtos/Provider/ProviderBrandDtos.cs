using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Core.Dtos.Provider
{
    public class ProviderBrandDetailsDto
    {
        public string Id { get; set; }

        public string Description { get; set; }
    }

    public class ProviderBrandInfo
    {
        public ProviderBrandDetailsDto Brand { get; set; }

        public IEnumerable<string> RedcetusIds { get; set; }
    }
}
