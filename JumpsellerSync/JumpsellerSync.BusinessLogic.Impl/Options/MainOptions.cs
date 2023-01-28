namespace JumpsellerSync.BusinessLogic.Impl.Options
{
    public class MainOptions
    {
        internal const string CONFIG_SECTION = "Main";

        public string HostedUrl { get; set; }

        public string VtexSecret { get; set; }
        public string VtexToken { get; set; }

        public string VtexAccountName { get; set; }
        public string VtexApiAccountName { get; set; }
        public string VtexWarehouseId { get; set; }
    }
}
