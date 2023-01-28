using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Main;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;

namespace JumpsellerSync.DataAccess.Impl.DbContexts
{
    public class MainNpgsqlDbContext : BaseDbContext
    {
        public MainNpgsqlDbContext(DbContextOptions<MainNpgsqlDbContext> options)
            : base(options)
        { }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<LocalProduct> LocalProducts { get; set; }

        public virtual DbSet<Brand> Brands { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<BaseProvider> Providers { get; set; }

        public virtual DbSet<SynchronizationSession> SynchronizationSessions { get; set; }

        public virtual DbSet<MainConfiguration> MainConfiguration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new NpgsqlProductConfiguration());
            modelBuilder.ApplyConfiguration(new NpgsqlLocalProductConfiguration());
            modelBuilder.ApplyConfiguration(new NpgsqlCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new NpgsqlBrandConfiguration());

            modelBuilder.ApplyConfiguration(new NpgsqlProviderConfiguration());
            modelBuilder.ApplyConfiguration(new NpgsqlHourlyProviderConfiguration());

            modelBuilder.ApplyConfiguration(new NpgsqlSynchronizationSessionConfiguration());

            modelBuilder.ApplyConfiguration(new NpgsqlMainConfiguration());
        }
    }


}
