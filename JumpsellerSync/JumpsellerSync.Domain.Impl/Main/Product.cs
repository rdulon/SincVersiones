using System.Collections.Generic;

namespace JumpsellerSync.Domain.Impl.Main
{
    public class Product : DomainModel
    {
        public virtual int? JumpsellerId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

        public virtual double Price { get; set; }

        public virtual int Margin { get; set; }

        public virtual double Stock { get; set; }

        public virtual string SKU { get; set; }

        public virtual string BrandId { get; set; }
        public virtual Brand Brand { get; set; }

        public virtual ICollection<string> ImageUrls { get; set; } = new HashSet<string>();

        public virtual ProductFormat Format { get; set; }

        public virtual double Weight { get; set; }

        public virtual double Width { get; set; }

        public virtual double Height { get; set; }

        public virtual double Length { get; set; }

        public virtual bool IsDigital { get; set; }

        public virtual bool SynchronizedToJumpseller { get; set; }

        public virtual ICollection<string> SynchronizingProviderIds { get; set; } = new HashSet<string>();

        public virtual LocalProduct LocalProduct { get; set; }
    }
}
