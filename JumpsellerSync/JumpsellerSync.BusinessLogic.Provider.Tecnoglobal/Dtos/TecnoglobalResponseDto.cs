using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Dtos
{
    internal class TecnoglobalResponseDto
    {
        public bool Error { get; set; }

        public string Message { get; set; }

        public IEnumerable<TecnoglobalProductDto> Products { get; set; }
    }
}
