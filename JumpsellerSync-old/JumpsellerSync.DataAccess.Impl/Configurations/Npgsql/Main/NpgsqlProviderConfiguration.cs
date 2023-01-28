using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main
{
    internal class NpgsqlProviderConfiguration : IEntityTypeConfiguration<BaseProvider>
    {
        public void Configure(EntityTypeBuilder<BaseProvider> builder)
        {
            builder
                .ToTable("Providers")
                .HasDiscriminator<string>("SynchronizationType")
                .HasValue<HourlyProvider>("Hourly")
                .HasValue<PeriodicallyProvider>("Periodically");

            builder.HasKey(p => p.Id);
            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            // Url
            builder
                .Property(p => p.Url)
                .IsRequired();
            builder
                .HasIndex(p => p.Url)
                .IsUnique();
            // Name
            builder
                .Property(p => p.Name)
                .IsRequired();
            builder
                .HasIndex(p => p.Name)
                .IsUnique();
            // Priority
            builder
                .HasIndex(p => p.Priority)
                .IsUnique();
        }
    }
}
