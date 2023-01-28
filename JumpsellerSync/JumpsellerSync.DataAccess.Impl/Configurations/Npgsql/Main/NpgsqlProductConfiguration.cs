using JumpsellerSync.DataAccess.Impl.Extensions;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main
{
    internal class NpgsqlProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(p => p.SynchronizedToJumpseller)
                .HasDefaultValue(false);
            builder
                .HasIndex(p => p.SynchronizedToJumpseller);

            builder
                .HasIndex(p => p.JumpsellerId)
                .IsUnique();

            builder
                .Property(p => p.ImageUrls)
                .HasCollectionConversion()
                .WithValueComparer();

            builder
                .Property(p => p.SynchronizingProviderIds)
                .HasCollectionConversion()
                .WithValueComparer();

            builder
                .HasIndex(p => p.SKU)
                .IsUnique();
        }
    }
}
