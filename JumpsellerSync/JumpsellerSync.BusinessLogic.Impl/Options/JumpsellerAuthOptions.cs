namespace JumpsellerSync.BusinessLogic.Impl.Options
{
    public class JumpsellerAuthOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string BaseUrl { get; set; }

        public JumpsellerAuthEndpoints Endpoints { get; set; }

        public string[] RequestScopes { get; set; }
    }

    public class JumpsellerAuthEndpoints
    {
        public string Authorize { get; set; }

        public string Token { get; set; }

        public string AuthorizeCallback { get; set; }
    }
}