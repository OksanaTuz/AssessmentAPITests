using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core;
using Core.DTOs.AccessToken;
using Core.DTOs.Company;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Test
{
    [TestClass]
    public class CompamnyTests
    {
        private static HttpClient _client;
        
        [ClassInitialize]
        public static async Task Setup(TestContext testContext)
        {
            var clientForGetToken = HttpClientCreator.CreateWebApiHttpClientForGetAccessToken();
            using (var requestMessage = new HttpRequestMessageCreator().PostRequestMessageForGetToken("/TestApi/token"))
                {
                    using (var response = await clientForGetToken.SendAsync(requestMessage).ConfigureAwait(false))
                    {
                        var result = await response.Content.ReadAsAsync<GetAccessTokenResponceDto>().ConfigureAwait(false);
                        var tokenType = result.token_type;
                        var accessToken = result.access_token;
                        _client = HttpClientCreator.CreateWebApiHttpClientWithToken(tokenType, accessToken);
                    }
                }
            clientForGetToken.Dispose();
         }

        [ClassCleanup]
        public static void Cleanup()
        {

            _client.Dispose();
        }


        [TestMethod]
        public async Task CreateCompany()
        {
            //arrange
            var companyName = TestUtils.GenerateString(10);
            using (var requestMessage = new HttpRequestMessageCreator().PostRequestMessageCreateCompany("/TestApi/api/automation/companies", companyName))
            {
                //act
                using (var responce = await _client.SendAsync(requestMessage)
                    .ConfigureAwait(false))
                {

                    //assert
                    Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                    Assert.IsTrue(responce.StatusCode.Equals(HttpStatusCode.OK), "Company isn't created");
                }
            }
        }

        [TestMethod]
        public async Task TryToCreateCompanyWithTheSameNameAsOneOfExisting()
        {
            //arrange
            var companyName = TestUtils.GenerateString(10);
            using (var requestMessage =
                new HttpRequestMessageCreator().PostRequestMessageCreateCompany("/TestApi/api/automation/companies", companyName))
            {
                using (var responce = await _client.SendAsync(requestMessage)
                    .ConfigureAwait(false))
                {
                    Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode,
                        "Internal server error");
                }
            }

            //act
            using (var requestMessage =
                new HttpRequestMessageCreator().PostRequestMessageCreateCompany("/TestApi/api/automation/companies", companyName))
            {
                using (var responce = await _client.SendAsync(requestMessage)
                    .ConfigureAwait(false))
                {
                    Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode,
                        "Internal server error");
             //assert
                    Assert.AreEqual(HttpStatusCode.BadRequest, responce.StatusCode);
                    var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Assert.IsTrue(result.Equals("Duplicate results found for identifier."), "Company duplicate is created");
                }
            }
        }

        [TestMethod]
        public async Task GetAllCompanies()
        {
            //arrange
            var companyName = TestUtils.GenerateString(10);
            using (var requestMessage =
                new HttpRequestMessageCreator().PostRequestMessageCreateCompany("/TestApi/api/automation/companies", companyName))
            {
                using (var responce = await _client.SendAsync(requestMessage)
                    .ConfigureAwait(false))
                {
                    Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode,
                        "Internal server error");
                }
            }

            //act
                using (var responce = await _client.GetAsync("/TestApi/api/automation/companies").ConfigureAwait(false))
            {

            //assert
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                Assert.IsFalse(result.Equals("[]"), "No companies");
                var company = JsonConvert.DeserializeObject<IList<CompanyDto>>(result).Last();
                Assert.IsTrue(company.Name.Equals(companyName));
            }
        }

        [TestMethod]
        public async Task GetCompanyById()
        {
            //arrange
            int companyId;
            string companyName;
            using ( var responce = await _client.GetAsync("/TestApi/api/automation/companies").ConfigureAwait(false))
            {

                //act
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                Assert.IsFalse(result.Equals("[]"), "No companies");
                var company = JsonConvert.DeserializeObject<IList<CompanyDto>>(result).FirstOrDefault();
                companyId = company.Id;
                companyName = company.Name;
            }

            using (var responce = await _client.GetAsync($"/TestApi/api/automation/companies/id/{companyId}").ConfigureAwait(false))
            {

                //assert
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                var company = JsonConvert.DeserializeObject<CompanyGetByIdResponceDto>(result);
                Assert.IsTrue(company.Name.Equals(companyName));
            }
            
        }

        [TestMethod]
        public async Task DeleteCompanyById()
        {
            //arrange
            int companyId;
            using (var responce = await _client.GetAsync("/TestApi/api/automation/companies").ConfigureAwait(false))
            {
                
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                Assert.IsFalse(result.Equals("[]"), "No companies");
                var company = JsonConvert.DeserializeObject<IList<CompanyDto>>(result).FirstOrDefault();
                companyId = company.Id;
            }

            //act
            using (var responce = await _client.DeleteAsync($"/TestApi/api/automation/companies/id/{companyId}").ConfigureAwait(false))
            {
                Assert.IsTrue(responce.StatusCode.Equals(HttpStatusCode.OK));
            }

            //assert
            using (var responce = await _client.GetAsync($"/TestApi/api/automation/companies/id/{companyId}").ConfigureAwait(false))
            {
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                Assert.IsTrue(responce.StatusCode.Equals(HttpStatusCode.NotFound));
            }

        }

        [TestMethod]
        public async Task DeleteCompanyByIdWithInvalidId()
        {
            var companyId = 100;
            var responce = await _client.DeleteAsync($"/TestApi/api/automation/companies/id/{companyId}").ConfigureAwait(false);
            Assert.IsTrue(responce.StatusCode.Equals(HttpStatusCode.NotFound));
        }
    }
}
