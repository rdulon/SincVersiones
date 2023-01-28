using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main
{
    internal class NpgsqlMainConfiguration : IEntityTypeConfiguration<MainConfiguration>
    {
        public void Configure(EntityTypeBuilder<MainConfiguration> builder)
        {
            builder.HasKey(config => config.Id);
            builder
                .Property(config => config.Id)
                .ValueGeneratedOnAdd();

            builder
                .OwnsOne(config => config.Jumpseller)
                .WithOwner();
        }
    }
}
