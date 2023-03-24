using System;

namespace JumpsellerSync.BusinessLogic.Core.Dtos.Main
{
    public class LoadProductsResultDto
    {
        public int Amount { get; set; }

        public TimeSpan LoadTime { get; set; }

        public string Message { get; set; }
    }
}
