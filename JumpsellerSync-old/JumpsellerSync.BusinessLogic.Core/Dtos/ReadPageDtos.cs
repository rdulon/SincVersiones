using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Core.Dtos
{
    public class ReadPageDto
    {
        public int Page { get; set; } = 1;

        public int Limit { get; set; } = 10;
    }


    public class ReadPageFilterDto : ReadPageDto
    {
        public string Filter { get; set; }
    }

    public class PageResultDto<TDetailsDto> : ReadPageDto
        where TDetailsDto : class
    {
        public IEnumerable<TDetailsDto> Items { get; set; }

        public int TotalPages { get; set; }
    }
}
