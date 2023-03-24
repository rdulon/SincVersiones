using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core;
using JumpsellerSync.DataAccess.Core.QueryModels.Main;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Impl.Repositories.Npgsql.Main
{
    public class SynchronizationSessionRepository
        : BaseRepository<SynchronizationSession, SynchronizationSessionRepository>,
          ISynchronizationSessionRepository
    {
        private readonly MainNpgsqlDbContext dbContext;

        public SynchronizationSessionRepository(
            MainNpgsqlDbContext dbContext,
            ILogger<SynchronizationSessionRepository> logger)
            : base(dbContext, logger)
        {
            this.dbContext = dbContext;
        }

        public IAsyncEnumerable<ReconcileProductInformation> GetReconcileInformationAsync(
            string sessionId)
        {
            var q = from session in dbContext.SynchronizationSessions.AsNoTracking()
                    where session.Id == sessionId &&
                          session.ProcessedDate == null
                    from info in session.Information
                    select new { session.ProviderId, info };

            return q
              .AsEnumerable()
              .GroupBy(r1 => r1.info.ProductId)
              .Select(g1 => new ReconcileProductInformation
              {
                  ProductId = g1.Key,
                  ProviderProducts = g1.Select(item => new ReconcileProviderProductInformation
                  {
                      ProviderId = item.ProviderId,
                      Stock = item.info.Stock,
                      Price = item.info.Price
                  })
              }).ToAsyncEnumerable();
        }

        public async Task<bool> IsSessionSynchronizedAsync(string sessionId)
        {
            var q = from session in dbContext.SynchronizationSessions.AsNoTracking()
                    where session.Id == sessionId && session.Running
                    select session;

            return await q.CountAsync() == 0;
        }

        public async Task<DataAccessResult<int>> MarkSessionProcessedAsync(
            string sessionId, bool deferSave = false)
        {
            var header = $"SynchronizationSessionRepository.SessionProcessed".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var now = DateTime.UtcNow;
                var sessions = await ReadAsync(s => s.Id == sessionId, 0, -1).ToListAsync();
                sessions.ForEach(session =>
                {
                    session.ProcessedDate = now;
                    session.UpdatedAt = now;
                    DbContext.Entry(session).State = EntityState.Modified;
                });

                return await AcknowledgedResultAsync(deferSave, header);
            }
            catch (Exception e)
            { logger.LogException(e, header); }
            return DataAccessResult.Fail<int>();
        }

        public override Task<DataAccessResult<SynchronizationSession>> CreateAsync(
            SynchronizationSession session, bool deferSave = false)
        {
            session.CreatedAt = DateTime.UtcNow;
            return base.CreateAsync(session, deferSave);
        }

        public override Task<DataAccessResult<SynchronizationSession>> UpdateAsync(
            SynchronizationSession session, bool deferSave = false, params string[] skipProperitesUpdate)
        {
            session.UpdatedAt = DateTime.UtcNow;
            return base.UpdateAsync(session, deferSave, skipProperitesUpdate);
        }

        public IAsyncEnumerable<BaseProvider> ReadSessionProvidersAsync(string sessionId)
        {
            return ReadAsync(
                session => session.Id == sessionId,
                session => session.Id, 0, -1,
                session => session.Provider);
        }

        public IAsyncEnumerable<string> GetSynchronizedSessionsAsync()
        {
            var q = from session in dbContext.SynchronizationSessions.AsNoTracking()
                    where session.ProcessedDate == null
                    group session by session.Id into g
                    select new
                    {
                        SessionId = g.Key,
                        All = g.Count(),
                        Finished = g.Sum(si => si.Running ? 1 : 0)
                    } into r
                    where r.All == r.Finished
                    select r.SessionId;

            return q.AsAsyncEnumerable();
        }
    }
}
