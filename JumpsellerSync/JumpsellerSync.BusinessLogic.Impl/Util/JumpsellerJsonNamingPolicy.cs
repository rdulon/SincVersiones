using System.Text.Json;
using System.Text.RegularExpressions;

namespace JumpsellerSync.BusinessLogic.Impl.Util
{
    internal class JumpsellerJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return Regex.Replace(name, "(.)([A-Z])", "$1_$2").ToLower();
        }
    }
}
