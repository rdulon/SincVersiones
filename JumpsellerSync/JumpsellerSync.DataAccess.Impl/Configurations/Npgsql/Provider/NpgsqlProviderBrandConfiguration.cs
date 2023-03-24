using JumpsellerSync.Domain.Impl.Provider;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider
{
    internal class NpgsqlProviderBrandConfiguration<TBrand>
        : IEntityTypeConfiguration<TBrand>
          where TBrand : ProviderBrand
    {
        public virtual void Configure(EntityTypeBuilder<TBrand> builder)
        {
            builder.HasKey(br => br.Id);
        }
    }
}
