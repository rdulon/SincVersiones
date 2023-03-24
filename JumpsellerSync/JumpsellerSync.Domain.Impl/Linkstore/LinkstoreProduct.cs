using JumpsellerSync.Domain.Impl.Provider;

using System;
using System.Collections.Generic;

namespace JumpsellerSync.Domain.Impl.Linkstore
{
    public class LinkstoreProduct : ProviderProduct
    {
        public virtual LinkstoreBrand Brand { get; set; }

        public virtual int AmountInTransit { get; set; }

        public virtual DateTime? InTransitArrival { get; set; }

        public virtual double ProviderPrice { get; set; }

        public virtual string CategoryId { get; set; }
        public virtual LinkstoreCategory Category { get; set; }

        public virtual string SubCategoryId { get; set; }
        public virtual LinkstoreSubcategory SubCategory { get; set; }

        public virtual double Weight { get; set; }

        public virtual ICollection<string> ImageUrls { get; set; } = new HashSet<string>();
    }
}
