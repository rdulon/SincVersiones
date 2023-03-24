using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Core.DbContexts;
using JumpsellerSync.DataAccess.Core.Repositories;
using JumpsellerSync.Domain.Impl;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Impl.Repositories
{
    public abstract class BaseRepository<TModel, TLogger>
        : IBaseRepository<TModel>,
          IUpsertRepository<TModel>
        where TModel : DomainModel
        where TLogger : class
    {

        protected readonly ILogger<TLogger> logger;

        public BaseRepository(BaseDbContext dbContext, ILogger<TLogger> logger)
        {
            DbContext = dbContext;
            this.logger = logger;
        }

        public BaseDbContext DbContext { get; }

        public virtual async Task<DataAccessResult<TModel>> CreateAsync(
            TModel domainModel, bool deferSave = false)
        {
            var header = $"Repository.Create".AsLogHeader();
            try
            {
                DbContext.Add(domainModel);

                if (!deferSave)
                {
                    logger.LogInformation($"{header} Save changes.");
                    await DbContext.SaveChangesAsync();
                }
                logger.LogInformation($"{header} End.");

                return DataAccessResult.Succeed(domainModel);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<TModel>();
        }

        public virtual async Task<DataAccessResult> DeleteAsync(TModel model, bool deferSave = false)
        {
            var header = $"Repository.Delete".AsLogHeader();
            try
            {
                logger.LogInformation($"{header} Init.");
                var entry = DbContext.ChangeTracker
                    .Entries<TModel>()
                    .FirstOrDefault(m => m.Entity.Id == model.Id);
                entry ??= DbContext.Entry(model);
                entry.State = EntityState.Deleted;

                if (!deferSave)
                {
                    logger.LogInformation($"{header} Save changes.");
                    await DbContext.SaveChangesAsync();
                }

                logger.LogInformation($"{header} End.");
                return DataAccessResult.Succeed();
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail();
        }

        public virtual async Task<DataAccessResult<TModel>> ReadAsync(object[] keys)
        {
            var header = $"Repository.Read".AsLogHeader();
            try
            {
                logger.LogInformation($"{header} Init.");
                var dbModel = await DbContext
                    .Set<TModel>()
                    .FindAsync(keys);

                logger.LogInformation($"{header} End.");
                return DataAccessResult.Succeed(dbModel);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<TModel>();
        }

        public IAsyncEnumerable<TProjection> ReadAsync<TOrderBy, TProjection>(
            Expression<Func<TModel, bool>> conditions,
            Expression<Func<TModel, TOrderBy>> orderBy,
            int offset, int limit,
            Expression<Func<TModel, TProjection>> projection)
        {
            var header = $"Repository.ReadMany".AsLogHeader();
            try
            {
                logger.LogInformation($"{header} Init.");

                IQueryable<TModel> q = DbContext.Set<TModel>()
                        .Where(conditions)
                        .OrderBy(orderBy);
                if (offset >= 0)
                { q = q.Skip(offset); }
                if (limit > 0)
                { q = q.Take(limit); }
                logger.LogInformation($"{header} End.");

                return q.Select(projection).AsAsyncEnumerable();
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return Enumerable.Empty<TProjection>().ToAsyncEnumerable();
        }

        public IAsyncEnumerable<TModel> ReadAsync<TOrderBy>(
                Expression<Func<TModel, bool>> conditions,
                Expression<Func<TModel, TOrderBy>> orderBy,
                int offset, int limit)
            => ReadAsync(conditions, orderBy, offset, limit, m => m);

        public async Task<int> GetPagesAsync(Expression<Func<TModel, bool>> conditions, int limit)
        {
            var count = await DbContext
                .Set<TModel>()
                .Where(conditions)
                .CountAsync();

            return (count / limit) + (count % limit == 0 ? 0 : 1);
        }

        public IAsyncEnumerable<TModel> ReadAsync(
                Expression<Func<TModel, bool>> conditions, int offset, int limit)
            => ReadAsync(conditions, m => m.Id, offset, limit);


        public virtual async Task<DataAccessResult<TModel>> UpdateAsync(
            TModel domainModel, bool deferSave = false, params string[] skipProperitesUpdate)
        {
            var header = $"Repository.Update".AsLogHeader();
            try
            {
                logger.LogInformation($"{header} Init. Create PK.");

                var entry = DbContext.ChangeTracker
                    .Entries<TModel>()
                    .FirstOrDefault(e => e.Entity.Id == domainModel.Id);
                entry ??= DbContext.Entry(domainModel);
                entry.State = EntityState.Modified;
                SkipPropertiesUpdate(entry, skipProperitesUpdate);

                if (!deferSave)
                {
                    logger.LogInformation($"{header} Save changes.");
                    await DbContext.SaveChangesAsync();
                }
                logger.LogInformation($"{header} End.");

                return DataAccessResult.Succeed(domainModel);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<TModel>();
        }

        public async Task<DataAccessResult<int>> UpsertAsync(
           IEnumerable<TModel> models, bool deferSave = false, params string[] skipProperitesUpdate)
        {
            var header = $"Repository.Upsert".AsLogHeader();
            try
            {
                logger.LogInformation($"{header} Init.");
                var modelIds = models.Select(m => m.Id).ToHashSet();
                var dbIds = new HashSet<string>(
                       await ReadAsync(m => modelIds.Contains(m.Id), m => m.Id, 0, -1, m => m.Id)
                           .ToListAsync());
                logger.LogInformation($"{header} Ids loaded.");

                foreach (var model in models)
                {
                    var entry = DbContext.Entry(model);
                    entry.State = dbIds.Contains(model.Id)
                        ? EntityState.Modified
                        : EntityState.Added;
                    SkipPropertiesUpdate(entry, skipProperitesUpdate);
                }

                return await AcknowledgedResultAsync(deferSave, header);
            }
            catch (Exception e)
            {
                logger.LogException(e, header);
                return DataAccessResult.Fail<int>();
            }
        }

        public async Task<DataAccessResult<int>> UpdateAsync(
            IEnumerable<TModel> domainModel, bool deferSave = false, params string[] skipProperitesUpdate)
        {
            var header = $"Repository.UpdateMany".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var tracked = DbContext.ChangeTracker
                    .Entries<TModel>()
                    .ToDictionary(e => e.Entity.Id);

                foreach (var model in domainModel)
                {
                    tracked.TryGetValue(model.Id, out var entry);
                    entry ??= DbContext.Entry(model);
                    entry.State = EntityState.Modified;
                    SkipPropertiesUpdate(entry, skipProperitesUpdate);
                }

                return await AcknowledgedResultAsync(deferSave, header);
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return DataAccessResult.Fail<int>();
        }

        protected async Task<DataAccessResult<int>> AcknowledgedResultAsync(bool deferSave, string header)
        {
            var acknowledged = 0;
            if (!deferSave)
            {
                logger.LogInformation($"{header} Save changes.");
                acknowledged = await DbContext.SaveChangesAsync();
            }
            logger.LogInformation($"{header} End");
            return DataAccessResult.Succeed(acknowledged);
        }

        private static void SkipPropertiesUpdate(EntityEntry<TModel> entry, string[] properties)
        {
            if (properties?.Length > 0)
            {
                foreach (var property in properties)
                { entry.Property(property).IsModified = false; }
            }
        }
    }
}
