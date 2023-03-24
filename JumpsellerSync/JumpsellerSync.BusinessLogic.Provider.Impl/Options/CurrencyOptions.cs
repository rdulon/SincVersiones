namespace JumpsellerSync.BusinessLogic.Provider.Impl.Options
{
    public class CurrencyOptions
    {
        public const string CONFIG_SECTION = "CurrencyOptions";

        public FreeCurrencyConverterOptions FreeCurrencyConverter { get; set; }

        public BCChOptions BCCh { get; set; }
    }
}
