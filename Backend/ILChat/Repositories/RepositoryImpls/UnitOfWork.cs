using ILChat.Entities;
using ILChat.Repositories.IRepositories;

namespace ILChat.Repositories.RepositoryImpls;

public class UnitOfWork(ChatDbContext context) : IUnitOfWork
{
    private readonly ChatDbContext _context = context;
    private bool _disposed;
    
    // Using a dictionary to store repositories
    private readonly Dictionary<Type, object> _repositories = new();

    public IRepository<T> Repository<T>() where T : class
    {
        // Check if repository exists
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IRepository<T>)_repositories[typeof(T)];
        }

        // Create a new repository
        var repository = new EfRepository<T>(_context);
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    // Implement SaveChanges method
    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    // Public Dispose method
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Suppress finalizer since we disposed manually
    }

    // Protected dispose method to follow Dispose pattern
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _context.Dispose();
            }

            // Dispose unmanaged resources here if any

            _disposed = true;
        }
    }

    // Finalizer to catch cases where Dispose was not called
    ~UnitOfWork()
    {
        Dispose(false);
    }
}