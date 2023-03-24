using JumpsellerSync.Domain.Impl.Provider;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider
{
    internal class NpgsqlProviderProductConfiguration<TProduct>
        : IEntityTypeConfiguration<TProduct>
          where TProduct : ProviderProduct
    {
        public virtual void Configure(EntityTypeBuilder<TProduct> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProductCode)
                .IsRequired();
            builder
                .HasIndex(p => p.ProductCode)
                .IsUnique();
            builder
                .HasIndex(p => p.RedcetusProductId)
                .IsUnique();
        }
    }
}
