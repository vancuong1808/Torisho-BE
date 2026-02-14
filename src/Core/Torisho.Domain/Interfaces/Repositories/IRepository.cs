using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Common;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : class, IAggregateRoot
{
    // Get entity by unique identifier
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Get all entities
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);

    // Add new entity
    Task AddAsync(T entity, CancellationToken ct = default);

    // Update existing entity
    Task UpdateAsync(T entity, CancellationToken ct = default);

    // Delete entity
    Task DeleteAsync(T entity, CancellationToken ct = default);
}
