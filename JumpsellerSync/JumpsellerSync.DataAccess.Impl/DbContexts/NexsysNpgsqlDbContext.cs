using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Nexsys;
using JumpsellerSync.DataAccess.Impl.Configurations.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Nexsys;

using Microsoft.EntityFrameworkCore;

namespace JumpsellerSync.DataAccess.Impl.DbContexts
{
    public class NexsysNpgsqlDbContext
        : ProviderDbContext<NexsysProduct, NexsysConfiguration>
    {
        public NexsysNpgsqlDbContext(DbContextOptions<NexsysNpgsqlDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(
                  new NpgsqlNexsysBrandConfiguration());
            modelBuilder.ApplyConfiguration(
                new NpgsqlProviderCategoryConfiguration<NexsysCategory>());

            modelBuilder.ApplyConfiguration(new NpgsqlProviderProductConfiguration<NexsysProduct>());
        }

        public virtual DbSet<NexsysBrand> Brands { get; set; }

        public virtual DbSet<NexsysCategory> Categories { get; set; }
    }
}
