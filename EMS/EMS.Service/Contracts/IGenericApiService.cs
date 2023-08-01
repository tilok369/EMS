namespace EMS.Service.Contracts;

public interface IGenericApiService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(string queryString);
    Task<T> GetAsync(string queryString);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(long id, T entity);
    Task<T> DeleteAsync(long id);
}
