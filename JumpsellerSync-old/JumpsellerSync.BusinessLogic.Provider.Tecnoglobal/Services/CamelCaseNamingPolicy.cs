using System.Text.Json;

namespace JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Services
{
    internal class CamelCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return $"{name[0]}".ToLower() + name.Substring(1);
        }
    }
}