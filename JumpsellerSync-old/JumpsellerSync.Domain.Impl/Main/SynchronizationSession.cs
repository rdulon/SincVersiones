using System;
using System.Collections.Generic;

namespace JumpsellerSync.Domain.Impl.Main
{
    public class SynchronizationSession : DomainModel
    {
        public virtual string ProviderId { get; set; }
        public virtual BaseProvider Provider { get; set; }

        public virtual bool Running { get; set; }

        public virtual DateTime? ProcessedDate { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual DateTime UpdatedAt { get; set; }

        public virtual ICollection<SynchronizationSessionInfo> Information { get; set; }
            = new HashSet<SynchronizationSessionInfo>();
    }

    public class SynchronizationSessionInfo
    {
        public virtual string ProductId { get; set; }

        public virtual double Stock { get; set; }

        public virtual double Price { get; set; }
    }
}
