using ILChat.Repositories.IRepositories;
using MongoDB.Driver;

namespace ILChat.Repositories.RepositoryImpls;

public class MongoRepository<T> : IMongoRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }
    
    public async Task<List<T>> GetAllAsync()
    {
        return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
            throw new Exception("Entity must have an 'Id' property");

        var idValue = idProperty.GetValue(entity)?.ToString();
        if (string.IsNullOrEmpty(idValue))
            throw new Exception("Entity Id must not be null or empty");

        var filter = Builders<T>.Filter.Eq("Id", idValue);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }
}