using EMS.Model;
using EMS.Service.Contracts;
using Newtonsoft.Json;
using System.Text;

namespace EMS.Service.Services
{
    public class EmployeeManagementService: IEmployeeManagementService
    {
        private readonly string _rootUrl;
        private readonly string _token;

        public EmployeeManagementService(string rootUrl, string token)
        {
            _token = token;
            _rootUrl = rootUrl;
        }

        public void GetEmployees()
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://gorest.co.in/public/v2/users?page=1&per_page=20 "),
                    Method = HttpMethod.Get,
                };
                var response = httpClient.SendAsync(request).GetAwaiter().GetResult();
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var employees = JsonConvert.DeserializeObject<IEnumerable<Employee>>(result);
            }

        }

        public void CreateEmployee()
        {
            using (var httpClient = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(new Employee
                {
                    name = "Tanjeer",
                    email = "tanjeer@gmail.com",
                    gender = "male",
                    status = "active"
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://gorest.co.in/public/v2/users"),
                    Method = HttpMethod.Post,
                    Content = content
                };
                request.Headers.Add("Authorization", "Bearer " + "0bf7fb56e6a27cbcadc402fc2fce8e3aa9ac2b40d4190698eb4e8df9284e2023");

                var response = httpClient.SendAsync(request).GetAwaiter().GetResult();
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var employee = JsonConvert.DeserializeObject<Employee>(result);
            }

        }

        public async Task<IEnumerable<Employee>> GetAllAsync(string queryString)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_rootUrl}{queryString}"),
                    Method = HttpMethod.Get,
                };
                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<IEnumerable<Employee>>(result);

                return employees;
            }
        }

        public async Task<Employee> GetAsync(string queryString)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_rootUrl}{queryString}"),
                    Method = HttpMethod.Get,
                };
                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(result);

                return employee;
            }
        }

        public async Task<Employee> CreateAsync(Employee entity)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(_rootUrl),
                    Method = HttpMethod.Post,
                    Content = ConstructContent(entity)
                };
                ConstructAuthHeader(request);

                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(result);

                return employee;
            }
        }      

        public async Task<Employee> UpdateAsync(long id, Employee entity)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_rootUrl}{id}"),
                    Method = HttpMethod.Put,
                    Content = ConstructContent(entity)
                };
                ConstructAuthHeader(request);

                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(result);

                return employee;
            }
        }

        public async Task<Employee> DeleteAsync(long id)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_rootUrl}{id}"),
                    Method = HttpMethod.Delete
                };
                ConstructAuthHeader(request);

                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(result);

                return employee;
            }
        }

        private void ConstructAuthHeader(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", $"Bearer {_token}");
        }

        private static StringContent ConstructContent(Employee entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }
    }
}