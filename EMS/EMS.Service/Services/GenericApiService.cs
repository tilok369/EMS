using EMS.Service.Contracts;
using Newtonsoft.Json;
using System.Text;

namespace EMS.Service.Services;

public class GenericApiService<T> : IGenericApiService<T> where T: class
{
    private readonly string _rootUrl;
    private readonly string _token;

    public GenericApiService(string rootUrl, string token)
    {
        _token = token;
        _rootUrl = rootUrl;
    }

    public async Task<IEnumerable<T>> GetAllAsync(string queryString)
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
            var entities = JsonConvert.DeserializeObject<IEnumerable<T>>(result);

            return entities;
        }
    }

    public async Task<T> GetAsync(string queryString)
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
            var entity = JsonConvert.DeserializeObject<T>(result);

            return entity;
        }
    }

    public async Task<T> CreateAsync(T entity)
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
            var createdEntity = JsonConvert.DeserializeObject<T>(result);

            return createdEntity;
        }
    }

    public async Task<T> UpdateAsync(long id, T entity)
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
            var updatedEntity = JsonConvert.DeserializeObject<T>(result);

            return updatedEntity;
        }
    }

    public async Task<T> DeleteAsync(long id)
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
            var deletedEntity = JsonConvert.DeserializeObject<T>(result);

            return deletedEntity;
        }
    }




    #region [Private Methods]

    private void ConstructAuthHeader(HttpRequestMessage request)
    {
        request.Headers.Add("Authorization", $"Bearer {_token}");
    }

    private static StringContent ConstructContent(T entity)
    {
        string json = JsonConvert.SerializeObject(entity);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }

    #endregion
}
