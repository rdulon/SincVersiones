using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider;
using JumpsellerSync.DataAccess.Impl.Extensions;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Linkstore
{
    internal class NpgsqlLinkstoreProductConfiguration
        : NpgsqlProviderProductConfiguration<LinkstoreProduct>
    {
        public override void Configure(EntityTypeBuilder<LinkstoreProduct> builder)
        {
            base.Configure(builder);

            builder
                .Property(p => p.ImageUrls)
                .HasCollectionConversion()
                .WithValueComparer();
        }
    }
}
