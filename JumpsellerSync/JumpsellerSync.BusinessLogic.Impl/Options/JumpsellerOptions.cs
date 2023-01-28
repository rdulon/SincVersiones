namespace JumpsellerSync.BusinessLogic.Impl.Options
{
    public class JumpsellerOptions
    {
        internal const string CONFIG_SECTION = "Jumpseller";

        public JumpsellerAuthOptions Auth { get; set; }

        public JumpsellerApiOptions Api { get; set; }
    }
}
