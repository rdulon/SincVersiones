using JumpsellerSync.Domain.Impl.Provider;

using System.Collections.Generic;

namespace JumpsellerSync.Domain.Impl.Intcomex
{
    public class IntcomexCategory : ProviderCategory
    {
        public virtual ICollection<IntcomexCategory> SubCategories { get; set; }
    }
}