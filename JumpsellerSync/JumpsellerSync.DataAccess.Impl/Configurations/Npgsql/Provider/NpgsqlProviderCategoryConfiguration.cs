using JumpsellerSync.Domain.Impl.Provider;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider
{
    internal class NpgsqlProviderCategoryConfiguration<TCategory>
        : IEntityTypeConfiguration<TCategory>
          where TCategory : ProviderCategory
    {
        public virtual void Configure(EntityTypeBuilder<TCategory> builder)
        {
            builder.HasKey(c => c.Id);
        }
    }
}
