using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Core.Dtos.Main
{
    public class ProviderDto
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int Priority { get; set; }

        public ProviderType ProviderType { get; set; }

        public string StartTime { get; set; }

        public int? Interval { get; set; }

        public List<string> Hours { get; set; }
    }

    public enum ProviderType
    {
        HourlyProvider = 1,
        PeriodicallyProvider
    }

}
