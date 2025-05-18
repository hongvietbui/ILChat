using MongoDB.Driver;

namespace ILChat.Repositories.IRepositories;

public interface IMongoRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(string id);
    Task<List<T>> FindAsync(FilterDefinition<T> filter, SortDefinition<T>? sort = null);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
}