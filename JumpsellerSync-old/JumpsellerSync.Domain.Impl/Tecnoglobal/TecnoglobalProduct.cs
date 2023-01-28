using JumpsellerSync.Domain.Impl.Provider;

using System;

namespace JumpsellerSync.Domain.Impl.Tecnoglobal
{
    public class TecnoglobalProduct : ProviderProduct
    {
        public virtual string Mpn { get; set; }

        public virtual string UpcEan { get; set; }

        public virtual TecnoglobalBrand Brand { get; set; }

        public virtual TecnoglobalCategory Category { get; set; }

        public virtual TecnoglobalSubcategory Subcategory { get; set; }

        public bool Offer { get; set; }

        public string CurrencyType { get; set; }

        public double TecnoglobalDollar { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
