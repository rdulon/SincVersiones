using JumpsellerSync.Domain.Impl.Provider;

using System;

namespace JumpsellerSync.Domain.Impl.Intcomex
{
    public class IntcomexConfiguration : ProviderConfiguration
    {
        public DateTime LastCatalogUpdate { get; set; }
    }
}
