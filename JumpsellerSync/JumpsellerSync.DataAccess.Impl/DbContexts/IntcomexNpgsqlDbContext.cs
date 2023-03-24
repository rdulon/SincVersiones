using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Intcomex;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Intcomex;

using Microsoft.EntityFrameworkCore;

namespace JumpsellerSync.DataAccess.Impl.DbContexts
{
    public class IntcomexNpgsqlDbContext
        : ProviderDbContext<IntcomexProduct, IntcomexConfiguration>
    {
        public IntcomexNpgsqlDbContext(
            DbContextOptions<IntcomexNpgsqlDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(
                new NpgsqlProviderBrandConfiguration<IntcomexBrand>());
            modelBuilder.ApplyConfiguration(
                new NpgsqlProviderCategoryConfiguration<IntcomexCategory>());
            modelBuilder.ApplyConfiguration(new NpgsqlIntcomexProductConfiguration());
        }

        public virtual DbSet<IntcomexBrand> Brands { get; set; }

        public virtual DbSet<IntcomexCategory> Categories { get; set; }
    }
}
