using ILChat.Entities;
using Microsoft.EntityFrameworkCore;

namespace ILChat.Entities;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
}