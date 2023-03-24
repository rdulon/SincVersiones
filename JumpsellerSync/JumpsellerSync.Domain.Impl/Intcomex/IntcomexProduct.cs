using JumpsellerSync.Domain.Impl.Provider;

namespace JumpsellerSync.Domain.Impl.Intcomex
{
    public class IntcomexProduct : ProviderProduct
    {
        public virtual string Mpn { get; set; }

        public virtual IntcomexBrand Brand { get; set; }

        public virtual IntcomexCategory Category { get; set; }

        public virtual IntcomexProductType ProductType { get; set; }

        public virtual bool Dirty { get; set; }

        public virtual string IntcomexSku { get; set; }
    }
}
