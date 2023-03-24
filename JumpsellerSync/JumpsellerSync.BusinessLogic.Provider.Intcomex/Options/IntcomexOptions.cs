namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.Options
{
    public class IntcomexOptions
    {
        public const string CONFIG_SECTION = "Intcomex";

        public AuthOptions Auth { get; set; }

        public ApiOptions Api { get; set; }
    }
}
