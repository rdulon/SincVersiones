using System;
using System.Collections.Generic;
using System.Linq;

namespace JumpsellerSync.Domain.Impl.Main
{
    public abstract class BaseProvider : DomainModel
    {
        public virtual string Url { get; set; }

        public virtual string Name { get; set; }

        public virtual bool Active { get; set; }

        public virtual int Priority { get; set; }

        public virtual DateTime? NextSynchronization { get; set; }

        public abstract void CalculateNextRun();
    }

    public class PeriodicallyProvider : BaseProvider
    {
        public virtual TimeSpan StartTime { get; set; }

        public virtual int Interval { get; set; }

        public override void CalculateNextRun()
        {
            var now = DateTime.UtcNow;

            if (NextSynchronization == null)
            { NextSynchronization = now.Date.Add(StartTime); }
            else
            {
                NextSynchronization = NextSynchronization.Value.AddHours(Interval);
                if (NextSynchronization.Value.Day > now.Day)
                { NextSynchronization = NextSynchronization.Value.Date.Add(StartTime); }
            }
        }
    }

    public class HourlyProvider : BaseProvider
    {
        public virtual ICollection<Hour> Hours { get; set; } = new HashSet<Hour>();

        public override void CalculateNextRun()
        {
            static IOrderedEnumerable<Hour> HoursOrdered(IEnumerable<Hour> hours)
                => hours
                    .OrderBy(h => h.Time.Hours)
                    .ThenBy(h => h.Time.Minutes);

            if (Hours.Count == 0)
            { NextSynchronization = null; }

            var now = DateTime.UtcNow;

            if (NextSynchronization == null)
            { NextSynchronization = now.Date.AddHours(23).AddMinutes(59).AddSeconds(59); }

            var hour = HoursOrdered(
                    Hours.Where(h => now.Date.Add(h.Time) > NextSynchronization.Value))
                .FirstOrDefault();

            var minHour = HoursOrdered(Hours).First();

            NextSynchronization = hour == null
               ? NextSynchronization.Value.AddDays(1).Date.Add(minHour.Time)
               : NextSynchronization.Value.Add(hour.Time);
        }
    }

    public class Hour
    {
        public Hour()
        { }

        public Hour(string hourSpec)
        {
            Time = TimeSpan.ParseExact(hourSpec, @"%h\:%m", default);
        }

        public virtual TimeSpan Time { get; set; }
    }
}
