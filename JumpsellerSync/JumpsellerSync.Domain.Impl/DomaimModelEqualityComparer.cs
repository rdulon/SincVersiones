using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JumpsellerSync.Domain.Impl
{
    public class DomainModelEqualityComparer<TDomainModel> : IEqualityComparer<TDomainModel>
        where TDomainModel : DomainModel
    {
        public bool Equals([AllowNull] TDomainModel x, [AllowNull] TDomainModel y)
        {
            return x != null && y != null && x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] TDomainModel obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
