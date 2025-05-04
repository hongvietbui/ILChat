using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ILChat.Utilities;

public class GetAllQueryOptions<T>
{
    public required Expression<Func<T, bool>> Filter { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public Func<IQueryable<T>, IIncludableQueryable<T, object>>? Include { get; set; }
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
    public bool DisableTracking { get; set; } = true;
}