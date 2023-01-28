using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main
{
    internal class NpgsqlSynchronizationSessionConfiguration
        : IEntityTypeConfiguration<SynchronizationSession>
    {
        public void Configure(EntityTypeBuilder<SynchronizationSession> builder)
        {
            builder.HasKey(ss => new { ss.Id, ss.ProviderId });
            builder.OwnsMany(ss => ss.Information, nb =>
            {
                nb.WithOwner().HasForeignKey("SynchronizationSessionId", "ProviderId");
                nb.Property<int>("SyncSessionInfoId");
                nb.HasKey("SyncSessionInfoId");
                nb.HasIndex(i => i.ProductId);
            });
        }
    }
}
