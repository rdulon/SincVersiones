using JumpsellerSync.Domain.Impl.Provider;

namespace JumpsellerSync.Domain.Impl.Nexsys
{
    public class NexsysCategory : ProviderCategory
    {
        public virtual NexsysCategory SubCategory { get; set; }
    }

}