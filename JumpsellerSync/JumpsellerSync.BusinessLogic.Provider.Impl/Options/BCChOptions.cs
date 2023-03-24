using System.Linq;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Options
{
    public class BCChOptions
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public SerieInfo[] Series { get; set; }

        public (string, double) GetSerie(string key)
        {
            return Series
                .Where(s => string.Compare(s.Key, key, true) == 0)
                .Select(s => (s.Value, s.Additive))
                .FirstOrDefault();
        }
    }

    public class SerieInfo
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public double Additive { get; set; }
    }
}