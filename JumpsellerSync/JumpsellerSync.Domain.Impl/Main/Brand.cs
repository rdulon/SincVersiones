namespace JumpsellerSync.Domain.Impl.Main
{
    public class Brand : DomainModel
    {
        public virtual string Name { get; set; }

        public virtual string NormalizedName { get; set; }
    }
}