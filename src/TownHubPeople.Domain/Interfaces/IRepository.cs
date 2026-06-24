using TownHubPeople.Domain.Common;

namespace TownHubPeople.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Query();
    IQueryable<T> QueryNoTracking();
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
