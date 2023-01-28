using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Tecnoglobal;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

using Microsoft.EntityFrameworkCore;

namespace JumpsellerSync.DataAccess.Impl.DbContexts
{
    public class TecnoglobalNpgsqlDbContext
        : ProviderDbContext<TecnoglobalProduct, TecnoglobalConfiguration>
    {
        public TecnoglobalNpgsqlDbContext(
            DbContextOptions<TecnoglobalNpgsqlDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(
                  new NpgsqlProviderBrandConfiguration<TecnoglobalBrand>());
            modelBuilder.ApplyConfiguration(
                new NpgsqlProviderCategoryConfiguration<TecnoglobalCategory>());
            modelBuilder.ApplyConfiguration(
                new NpgsqlProviderCategoryConfiguration<TecnoglobalSubcategory>());

            modelBuilder.ApplyConfiguration(new NpgsqlTecnoglobalProductConfiguration());
        }

        public virtual DbSet<TecnoglobalBrand> Brands { get; set; }

        public virtual DbSet<TecnoglobalCategory> Categories { get; set; }

        public virtual DbSet<TecnoglobalSubcategory> Subcategories { get; set; }

    }
}
