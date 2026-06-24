using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Infrastructure.Data;

namespace TownHubPeople.Infrastructure.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public IQueryable<T> Query() => _dbSet;
    public IQueryable<T> QueryNoTracking() => _dbSet.AsNoTracking();

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbSet.FindAsync([id], ct);

    public void Add(T entity) => _dbSet.Add(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Remove(T entity) => _dbSet.Remove(entity);
}
