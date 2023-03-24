using Hangfire.Common;
using Hangfire.Server;

using System;

namespace JumpsellerSync.Common.Util.Hangfire
{
    public class SkipConcurrentJobAttribute : JobFilterAttribute, IServerFilter
    {
        private const string lockName = "SkipConcurrentJobLock";
        private readonly TimeSpan lockTimeout;

        public SkipConcurrentJobAttribute(int lockTimeoutInSeconds = 2)
        { lockTimeout = TimeSpan.FromSeconds(lockTimeoutInSeconds); }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (!filterContext.Items.ContainsKey(lockName))
            { throw new InvalidOperationException($"Lock {lockName} not found."); }

            using var @lock = (IDisposable)filterContext.Items[lockName];
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var jobId = filterContext.BackgroundJob.Id;
            try
            {
                var @lock = filterContext.Connection.AcquireDistributedLock(jobId, lockTimeout);
                filterContext.Items[lockName] = @lock;
            }
            catch
            { filterContext.Canceled = true; }
        }
    }
}
