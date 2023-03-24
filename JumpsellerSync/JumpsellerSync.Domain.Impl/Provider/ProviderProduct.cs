namespace JumpsellerSync.Domain.Impl.Provider
{
    public abstract class ProviderProduct : DomainModel
    {
        public virtual string RedcetusProductId { get; set; }

        public virtual string BrandId { get; set; }

        public virtual double Price { get; set; }

        public virtual double Stock { get; set; }

        public virtual string ProductCode { get; set; }

        public virtual string Description { get; set; }

    }
}
