using MongoDB.Domain.Entities;
using System.Linq.Expressions;

namespace MongoDB.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(string collectionName);
    Task<T?> GetByIdAsync(string id, string collectionName);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, string collectionName);
    Task<T> CreateAsync(T entity, string collectionName);
    Task<T> UpdateAsync(T entity, string collectionName);
    Task<bool> DeleteAsync(string id, string collectionName);
    Task<long> CountAsync(string collectionName);
    Task<long> CountAsync(Expression<Func<T, bool>> filter, string collectionName);
}