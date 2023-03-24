using System.Text.Json;

namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.Services
{
    internal class AsIsNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name;
        }
    }
}