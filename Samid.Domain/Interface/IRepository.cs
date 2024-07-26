using System.Linq.Expressions;

namespace Samid.Domain.Interface;

public interface IRepository<T> where T : class
{
  Task<T?> GetByIdAsync(Guid id);
  Task<IEnumerable<T>> GetAllAsync();
  Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
  IQueryable<T> GetQueryable();
  Task<int> CountAsync(Expression<Func<T, bool>> predicate);
  Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
  Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
  Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
  Task AddAsync(T entity);
  Task AddRangeAsync(IEnumerable<T> entities);
  Task UpdateAsync(T entity);
  Task UpdateRangeAsync(IEnumerable<T> entities);
  Task RemoveAsync(T entity);
  Task RemoveRangeAsync(IEnumerable<T> entities);
  Task<T?> FindByIdAsync(Guid id);
}