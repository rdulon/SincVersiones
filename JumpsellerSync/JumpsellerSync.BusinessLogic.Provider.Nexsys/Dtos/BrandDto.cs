using System.Text.Json.Serialization;

namespace JumpsellerSync.BusinessLogic.Provider.Nexsys.Dtos
{
    internal class BrandDto
    {
        [JsonPropertyName("fabricante")]
        public string Manufacturer { get; set; }

        [JsonPropertyName("nexsysId")]
        public string NexsysId { get; set; }
    }
}
