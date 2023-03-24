namespace JumpsellerSync.Domain.Impl.Main
{
    public class LocalProduct : DomainModel
    {
        public virtual string Name { get; set; }

        public virtual string SKU { get; set; }

        public virtual double Price { get; set; }

        public virtual double Stock { get; set; }

        public virtual int JumpsellerId { get; set; }

        public virtual string ProductId { get; set; }
        public virtual Product Product { get; set; }

        public virtual string BrandId { get; set; }
        public virtual Brand Brand { get; set; }

    }
}
