using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Common;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class, IAggregatedRoot
{
    protected readonly IDataContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(IDataContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }
    
    // public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    // {
    //     return await _dbSet.ToListAsync(ct);
    // }
    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}