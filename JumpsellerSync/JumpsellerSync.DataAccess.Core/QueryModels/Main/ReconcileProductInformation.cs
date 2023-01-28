using System.Collections.Generic;

namespace JumpsellerSync.DataAccess.Core.QueryModels.Main
{
    public class ReconcileProductInformation
    {
        public string ProductId { get; set; }
        public string ProviderId { get; set; }

        public IEnumerable<ReconcileProviderProductInformation> ProviderProducts { get; set; }
    }

    public class ReconcileProviderProductInformation
    {
        public string ProviderId { get; set; }

        public double Stock { get; set; }

        public double Price { get; set; }
    }
}
