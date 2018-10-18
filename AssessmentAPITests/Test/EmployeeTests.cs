using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core;
using Core.DTOs.AccessToken;
using Core.DTOs.Employee;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Test
{
    [TestClass]
    public class EmployeeTests
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
        public async Task CreateEmployee()
        {
            //arrange
            var employeeName = TestUtils.GenerateString(10);
            using (var requestMessage = new HttpRequestMessageCreator().PostRequestMessageCreateEmployee("/TestApi/api/automation/employees", employeeName))
            {
                //act
                using (var responce = await _client.SendAsync(requestMessage)
                    .ConfigureAwait(false))
                {

                    //assert
                    Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                    Assert.IsTrue(responce.StatusCode.Equals(HttpStatusCode.OK), "Employee isn't created");
                }
            }
        }

        [TestMethod]
        public async Task TryToCreateEmployeeWithTheSameNameAsOneOfExisting()
        {
            //arrange
            var employeeName = TestUtils.GenerateString(10);
            using (var requestMessage =
                new HttpRequestMessageCreator().PostRequestMessageCreateEmployee("/TestApi/api/automation/employees", employeeName))
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
                new HttpRequestMessageCreator().PostRequestMessageCreateEmployee("/TestApi/api/automation/employees", employeeName))
            {
                using (var responce = await _client.SendAsync(requestMessage)
                    .ConfigureAwait(false))
                {
                    Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode,
                        "Internal server error");
                    //assert
                    Assert.AreEqual(HttpStatusCode.BadRequest, responce.StatusCode);
                    var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Assert.IsTrue(result.Equals("Duplicate results found for identifier."), "Employee duplicate is created");
                }
            }
        }

        [TestMethod]
        public async Task GetAllEmployees()
        {
            //arrange
            var employeeName = TestUtils.GenerateString(10);
            using (var requestMessage =
                new HttpRequestMessageCreator().PostRequestMessageCreateEmployee("/TestApi/api/automation/employees", employeeName))
            {
                using (var responce = await _client.SendAsync(requestMessage)
                    .ConfigureAwait(false))
                {
                    Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode,
                        "Internal server error");
                }
            }

            //act
            using (var responce = await _client.GetAsync("/TestApi/api/automation/employees").ConfigureAwait(false))
            {

                //assert
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                Assert.IsFalse(result.Equals("[]"), "No employees");
                var employee = JsonConvert.DeserializeObject<List<EmployeeDto>>(result).Last();
                Assert.IsTrue(employee.Name.Equals(employeeName));
            }
        }

        [TestMethod]
        public async Task GetEmployeeById()
        {
            //arrange
            int employeeId;
            string employeeName;
            using (var responce = await _client.GetAsync("/TestApi/api/automation/employees").ConfigureAwait(false))
            {

                //act
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                Assert.IsFalse(result.Equals("[]"), "No employees");
                var employee = JsonConvert.DeserializeObject<List<EmployeeDto>>(result).Last();
                employeeId = employee.Id;
                employeeName = employee.Name;
            }

            using (var responce = await _client.GetAsync($"/TestApi/api/automation/employees/id/{employeeId}").ConfigureAwait(false))
            {

                //assert
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                var employee = JsonConvert.DeserializeObject<EmployeeGetByIdResponceDto>(result);
                Assert.IsTrue(employee.Name.Equals(employeeName));
            }

        }

        [TestMethod]
        public async Task DeleteEmployeeById()
        {
            //arrange
            int employeeId;
            using (var responce = await _client.GetAsync("/TestApi/api/automation/employees").ConfigureAwait(false))
            {

                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                Assert.IsFalse(result.Equals("[]"), "No eployees");
                var employee = JsonConvert.DeserializeObject<List<EmployeeDto>>(result).FirstOrDefault();
                employeeId = employee.Id;
            }

            //act
            using (var responce = await _client.DeleteAsync($"/TestApi/api/automation/employees/id/{employeeId}").ConfigureAwait(false))
            {
                Assert.IsTrue(responce.StatusCode.Equals(HttpStatusCode.OK));
            }

            //assert
            using (var responce = await _client.GetAsync($"/TestApi/api/automation/employees/id/{employeeId}").ConfigureAwait(false))
            {
                Assert.AreNotEqual(HttpStatusCode.InternalServerError, responce.StatusCode, "Internal server error");
                Assert.IsTrue(responce.StatusCode.Equals(HttpStatusCode.NotFound));
            }

        }

        [TestMethod]
        public async Task DeleteEmployeeByIdWithInvalidId()
        {
            var employeeId = 1000;
            var responce = await _client.DeleteAsync($"/TestApi/api/automation/employees/id/{employeeId}").ConfigureAwait(false);
            Assert.IsTrue(responce.StatusCode.Equals(HttpStatusCode.NotFound));
        }
    }
}
