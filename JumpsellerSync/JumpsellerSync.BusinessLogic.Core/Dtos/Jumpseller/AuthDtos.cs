﻿namespace JumpsellerSync.BusinessLogic.Core.Dtos.Jumpseller
{
    public class AuthTokensDto
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }

        public string RefreshToken { get; set; }

        public long CreatedAt { get; set; }

        public string Scope { get; set; }
    }
}
