using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Intcomex;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Intcomex
{
    internal class NpgsqlIntcomexProductConfiguration
        : NpgsqlProviderProductConfiguration<IntcomexProduct>
    {
        public override void Configure(EntityTypeBuilder<IntcomexProduct> builder)
        {
            base.Configure(builder);

            builder.HasIndex(p => p.Mpn);

            builder.HasIndex(p => p.IntcomexSku);
        }
    }
}
