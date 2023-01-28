using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Linkstore
{
    internal class NpgsqlLinkstoreSubCategoryConfiguration : IEntityTypeConfiguration<LinkstoreSubcategory>
    {
        public void Configure(EntityTypeBuilder<LinkstoreSubcategory> builder)
        {
            builder.HasKey(sc => sc.Id);
        }
    }
}
