using ILChat.Entities;
using ILChat.Repositories.IRepositories;

namespace ILChat.Repositories.RepositoryImpls;

public class UserRepositoryImpl : EfRepository<User>, IUserRepository
{
    private readonly ChatDbContext _context;

    public UserRepositoryImpl(ChatDbContext context) : base(context) { 
        _context = context;
    }
}