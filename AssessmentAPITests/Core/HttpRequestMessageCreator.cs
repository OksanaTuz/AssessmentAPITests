using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Core
{
    public class HttpRequestMessageCreator
    {
        public HttpRequestMessage PostRequestMessageForGetToken(string apiUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("username", "testName"));
            keyValues.Add(new KeyValuePair<string, string>("password", "test"));
            keyValues.Add(new KeyValuePair<string, string>("grant_type","password"));
            request.Content = new FormUrlEncodedContent(keyValues);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            return request;
        }
    }
}
