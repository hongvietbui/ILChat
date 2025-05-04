namespace ILChat.Repositories.IRepositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task SaveChangesAsync();
    void SaveChanges();
}