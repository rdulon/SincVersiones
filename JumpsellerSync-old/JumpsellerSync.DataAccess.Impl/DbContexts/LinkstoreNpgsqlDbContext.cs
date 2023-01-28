using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Linkstore;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.EntityFrameworkCore;

namespace JumpsellerSync.DataAccess.Impl.DbContexts
{
    public class LinkstoreNpgsqlDbContext : ProviderDbContext<LinkstoreProduct, LinkstoreConfiguration>
    {
        public LinkstoreNpgsqlDbContext(
            DbContextOptions<LinkstoreNpgsqlDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(
                new NpgsqlProviderBrandConfiguration<LinkstoreBrand>());
            modelBuilder.ApplyConfiguration(
                new NpgsqlProviderCategoryConfiguration<LinkstoreCategory>());
            modelBuilder.ApplyConfiguration(new NpgsqlLinkstoreSubCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new NpgsqlLinkstoreProductConfiguration());
        }

        public virtual DbSet<LinkstoreBrand> Brands { get; set; }

        public virtual DbSet<LinkstoreCategory> Categories { get; set; }

        public virtual DbSet<LinkstoreSubcategory> SubCategories { get; set; }
    }
}
