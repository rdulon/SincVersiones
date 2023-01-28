using JumpsellerSync.Domain.Impl.Provider;

namespace JumpsellerSync.Domain.Impl.Nexsys
{
    public class NexsysProduct : ProviderProduct
    {
        public string Currency { get; set; }

        public virtual NexsysBrand Brand { get; set; }

        public virtual NexsysCategory Category { get; set; }

        public virtual string ImageUrl { get; set; }

        public long Parent { get; set; }

        public string TaxExcluded { get; set; }
    }
}
