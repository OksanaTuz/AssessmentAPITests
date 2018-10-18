using System;
using System.Net.Http;

namespace Core
{
    public static class HttpClientCreator
    {
        public static HttpClient CreateWebApiHttpClientWithToken(string tokenType, string accessToken)
        {
            var handler = new WebRequestHandler
            {
                ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(Settings.ApiBasePath),
                DefaultRequestHeaders =
                {
                    { "Authorization", tokenType + " " + accessToken}
                }
            };
        }
        public static HttpClient CreateWebApiHttpClientForGetAccessToken()
        {
            var handler = new WebRequestHandler
            {
                ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            return new HttpClient(handler)
            {
                BaseAddress = new Uri(Settings.ApiBasePath)
            };
        }
    }
}
