using EMS.Model;

namespace EMS.Service.Contracts;

public interface IGenericApiService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(string queryString);
    Task<T> GetAsync(long id);
    Task<(T?, IEnumerable<ValidationMessage>?)> CreateAsync(T entity);
    Task<(T?, IEnumerable<ValidationMessage>?)> UpdateAsync(long id, T entity);
    Task<bool> DeleteAsync(long id);
}
