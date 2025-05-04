using System.Linq.Expressions;
using ILChat.Repositories.IRepositories;
using ILChat.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;

namespace ILChat.Repositories.RepositoryImpls;

public class EfRepository<T> : IDisposable, IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly ILogger _logger;

    public EfRepository(DbContext context)
    {
        _logger = context.GetService<ILogger<EfRepository<T>>>();
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }
    
    public IQueryable<T> GetAll()
    {
        return _dbSet;
    }

    public Pagination<T> GetAll(GetAllQueryOptions<T> options)
    {
        IQueryable<T> query = _dbSet;
        if (options.DisableTracking)
        {
            query = query.AsNoTracking();
        }
        
        if(options.Include != null)
            query = options.Include(query);
        
        if (options.OrderBy != null)
        {
            query = options.OrderBy(query);
        }

        var itemCount = query.Count(options.Filter);
        var item = query
            .Where(options.Filter)
            .Skip((options.PageIndex - 1) * options.PageSize)
            .Take(options.PageSize)
            .AsNoTracking()
            .ToList();

        return new Pagination<T>
        {
            PageIndex = options.PageIndex,
            PageSize = options.PageSize,
            TotalItems = itemCount,
            Items = item
        };
    }

    public async Task<Pagination<T>> GetAllAsync(GetAllQueryOptions<T> options)
    {
        IQueryable<T> query = _dbSet;
        if (options.DisableTracking)
        {
            query = query.AsNoTracking();
        }
        
        if(options.Include != null)
            query = options.Include(query);
        
        if (options.OrderBy != null)
        {
            query = options.OrderBy(query);
        }

        var itemCount = await query.CountAsync(options.Filter);
        var item = await query
            .Where(options.Filter)
            .Skip((options.PageIndex - 1) * options.PageSize)
            .Take(options.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new Pagination<T>
        {
            PageIndex = options.PageIndex,
            PageSize = options.PageSize,
            TotalItems = itemCount,
            Items = item
        };
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbSet.AnyAsync(filter);
    }

    public async Task<bool> AnyAsync()
    {
        return await _dbSet.AnyAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return (await _dbSet.FindAsync(id)) ?? null;
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool disableTracking = true)
    {
        IQueryable<T> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        query = query.IgnoreQueryFilters();
        
        return await query.FirstOrDefaultAsync(filter);
    }
    
    public async Task<IEnumerable<T>?> FindByConditionAsync(Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool disableTracking = true)
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }
            
            query = query.Where(filter);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + " "+ ex.StackTrace);
            throw;
        }
    }

    public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool disableTracking = true)
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }
            
            query = query.Where(filter);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + " "+ ex.StackTrace);
            throw;
        }
    }

    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbSet.CountAsync(filter);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task DeleteByIdAsync(object id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}