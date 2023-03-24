using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Tecnoglobal
{
    internal class NpgsqlTecnoglobalProductConfiguration
        : NpgsqlProviderProductConfiguration<TecnoglobalProduct>
    {
        public override void Configure(EntityTypeBuilder<TecnoglobalProduct> builder)
        {
            base.Configure(builder);

            builder
                .HasIndex(p => p.Mpn);

            builder
                .HasIndex(p => p.UpcEan);
        }
    }
}
