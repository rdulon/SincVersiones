using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Core.Dtos.Provider
{
    public class SynchronizationInfoDto
    {
        public bool SyncComplete { get; set; }

        public IEnumerable<SynchronizeProductDto> Products { get; set; }
    }
}
