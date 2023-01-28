using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Nexsys;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Nexsys
{
    internal class NpgsqlNexsysBrandConfiguration
        : NpgsqlProviderBrandConfiguration<NexsysBrand>
    {
        public override void Configure(EntityTypeBuilder<NexsysBrand> builder)
        {
            base.Configure(builder);

            builder.HasIndex(brand => brand.NexsysId).IsUnique();
        }
    }
}
