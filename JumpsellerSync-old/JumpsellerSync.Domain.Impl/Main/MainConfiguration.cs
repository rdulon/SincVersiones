using System;

namespace JumpsellerSync.Domain.Impl.Main
{
    public class MainConfiguration : DomainModel
    {
        public virtual JumpsellerConfiguration Jumpseller { get; set; }
    }

    public class JumpsellerConfiguration
    {
        public virtual bool ApplicationAuthorized { get; set; }

        public virtual string AccessToken { get; set; }

        public virtual string RefreshToken { get; set; }

        public virtual string AccessTokenType { get; set; }

        public virtual DateTime? TokenExpiresAt { get; set; }

        public virtual DateTime? TokenCreatedAt { get; set; }

        public virtual string Scope { get; set; }
    }
}
