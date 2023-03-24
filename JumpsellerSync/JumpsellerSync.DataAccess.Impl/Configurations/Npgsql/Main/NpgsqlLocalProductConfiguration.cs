using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main
{
    internal class NpgsqlLocalProductConfiguration : IEntityTypeConfiguration<LocalProduct>
    {
        public void Configure(EntityTypeBuilder<LocalProduct> builder)
        {
            builder.HasKey(p => p.Id);
            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasIndex(p => p.JumpsellerId)
                .IsUnique();

            builder
                .HasIndex(p => p.SKU)
                .IsUnique();

            builder
                .HasOne(lp => lp.Product)
                .WithOne(p => p.LocalProduct)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
