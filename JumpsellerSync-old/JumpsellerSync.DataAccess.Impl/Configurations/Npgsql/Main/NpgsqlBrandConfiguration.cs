using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main
{
    internal class NpgsqlBrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasKey(b => b.Id);
            builder
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();
            builder
                .Property(b => b.NormalizedName)
                .IsRequired();
            builder.HasIndex(b => b.NormalizedName);

        }
    }
}
