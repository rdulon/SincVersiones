using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main
{
    internal class NpgsqlHourlyProviderConfiguration : IEntityTypeConfiguration<HourlyProvider>
    {
        public void Configure(EntityTypeBuilder<HourlyProvider> builder)
        {
            builder.OwnsMany(hp => hp.Hours, nb =>
            {
                nb.WithOwner().HasForeignKey("HourlyProviderId");
                nb.Property<int>("HourId");
                nb.HasKey("HourId");
            });
        }
    }
}
