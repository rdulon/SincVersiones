using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main
{
    internal class NpgsqlCategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
            builder
                .Property(c => c.ProviderCategoryId)
                .IsRequired();
            builder.HasIndex(c => c.ProviderCategoryId);
        }
    }
}
