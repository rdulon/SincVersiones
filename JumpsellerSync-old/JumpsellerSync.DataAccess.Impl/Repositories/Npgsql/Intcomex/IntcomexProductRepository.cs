using JumpsellerSync.DataAccess.Core.Repositories.Intcomex;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Provider;
using JumpsellerSync.Domain.Impl.Intcomex;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Intcomex
{
    public class IntcomexProductRepository
        : ProviderProductRepository<IntcomexProduct, IntcomexBrand, IntcomexConfiguration, IntcomexProductRepository>,
          IIntcomexProductRepository

    {
        private readonly IntcomexNpgsqlDbContext dbContext;
        private readonly string productsTable;
        private readonly string dirtyColumn;

        public IntcomexProductRepository(
            IntcomexNpgsqlDbContext dbContext,
            ILogger<IntcomexProductRepository> logger)
            : base(dbContext, logger)
        {
            this.dbContext = dbContext;

            var productsEntityType = dbContext.Model.FindEntityType(typeof(IntcomexProduct));
            productsTable = productsEntityType.GetTableName();
            dirtyColumn = productsEntityType.FindProperty(nameof(IntcomexProduct.Dirty)).GetColumnName();
        }

        public async Task MarkProductsAsDirtyAsync()
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                $"UPDATE \"{productsTable}\" SET \"{dirtyColumn}\" = true;");
        }

        public async Task RemoveDirtyProductsAsync()
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                $"DELETE FROM \"{productsTable}\" WHERE \"{dirtyColumn}\" = true;");
        }
    }
}
