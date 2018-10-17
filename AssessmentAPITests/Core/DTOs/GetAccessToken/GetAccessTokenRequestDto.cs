namespace Core.DTOs.GetAccessToken
{
    public class GetAccessTokenRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Grand_Type { get; set; }
    }
}
