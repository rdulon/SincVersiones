using JumpsellerSync.Domain.Impl.Provider;

using Microsoft.EntityFrameworkCore;

namespace JumpsellerSync.DataAccess.Core.DbContexts
{
    public abstract class ProviderDbContext<TProduct, TConfiguration> : BaseDbContext
        where TProduct : ProviderProduct
        where TConfiguration : ProviderConfiguration
    {
        public ProviderDbContext(DbContextOptions options)
            : base(options)
        { }

        public virtual DbSet<TProduct> Products { get; set; }

        public virtual DbSet<TConfiguration> Configuration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var configBuilder = modelBuilder.Entity<TConfiguration>();
            configBuilder.HasKey(c => c.Id);
            configBuilder
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
