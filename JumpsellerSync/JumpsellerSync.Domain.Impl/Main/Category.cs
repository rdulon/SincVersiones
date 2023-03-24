namespace JumpsellerSync.Domain.Impl.Main
{
    public class Category : DomainModel
    {
        public virtual string Name { get; set; }

        public virtual string ProviderCategoryId { get; set; }
    }
}