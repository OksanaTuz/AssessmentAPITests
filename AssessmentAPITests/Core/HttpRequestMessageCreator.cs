using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Core.DTOs.Company;
using Core.DTOs.Employee;
using Newtonsoft.Json;

namespace Core
{
    public class HttpRequestMessageCreator
    {
        public HttpRequestMessage PostRequestMessageForGetToken(string apiUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", "testName"),
                new KeyValuePair<string, string>("password", "test"),
                new KeyValuePair<string, string>("grant_type", "password")
            };
            request.Content = new FormUrlEncodedContent(keyValues);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            return request;
        }

        public HttpRequestMessage PostRequestMessageCreateCompany(string apiUrl, string companyName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            var companyRequest = new CompanyCreateRequestDto
            {
                Name = companyName
            };
            var companyRequestJSon = JsonConvert.SerializeObject(companyRequest);
            var content = new StringContent(companyRequestJSon, Encoding.UTF8, "application/json");
            request.Content = content;
            return request;
        }

        public HttpRequestMessage PostRequestMessageCreateEmployee(string apiUrl, string employeeName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            var employeeRequest = new EmployeeCreateRequestDto
            {
                Name = employeeName
            };
            var companyRequestJSon = JsonConvert.SerializeObject(employeeRequest);
            var content = new StringContent(companyRequestJSon, Encoding.UTF8, "application/json");
            request.Content = content;
            return request;
        }
    }
}
