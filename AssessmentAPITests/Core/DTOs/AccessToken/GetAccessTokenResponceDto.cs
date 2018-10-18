using System;

namespace Core.DTOs.AccessToken
{
    public class GetAccessTokenResponceDto
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string displayName { get; set; }
        public DateTime issued { get; set; }
        public DateTime expirse { get; set; }

    }
}
