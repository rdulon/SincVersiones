namespace JumpsellerSync.BusinessLogic.Impl.Options
{
    public class JumpsellerApiOptions
    {
        public string BaseUrl { get; set; }

        public string Version { get; set; }

        public JumpsellerApiEndpoints Endpoints { get; set; }

        public int ReadPageSize { get; set; }
    }

    public class JumpsellerApiEndpoints
    {
        public string Products { get; set; }
    }
}