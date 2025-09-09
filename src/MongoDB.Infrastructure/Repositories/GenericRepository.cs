using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using MongoDB.Infrastructure.Data;
using System.Linq.Expressions;

namespace MongoDB.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly MongoDbContext _context;

    public GenericRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync(string collectionName)
    {
        var collection = _context.GetCollection<T>(collectionName);
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(string id, string collectionName)
    {
        var collection = _context.GetCollection<T>(collectionName);
        return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, string collectionName)
    {
        var collection = _context.GetCollection<T>(collectionName);
        return await collection.Find(filter).ToListAsync();
    }

    public async Task<T> CreateAsync(T entity, string collectionName)
    {
        var collection = _context.GetCollection<T>(collectionName);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        if (string.IsNullOrEmpty(entity.Id))
        {
            entity.Id = ObjectId.GenerateNewId().ToString();
        }

        await collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity, string collectionName)
    {
        var collection = _context.GetCollection<T>(collectionName);
        entity.UpdatedAt = DateTime.UtcNow;
        
        await collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(string id, string collectionName)
    {
        var collection = _context.GetCollection<T>(collectionName);
        var result = await collection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<long> CountAsync(string collectionName)
    {
        var collection = _context.GetCollection<T>(collectionName);
        return await collection.CountDocumentsAsync(_ => true);
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>> filter, string collectionName)
    {
        var collection = _context.GetCollection<T>(collectionName);
        return await collection.CountDocumentsAsync(filter);
    }
}