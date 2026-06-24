using Microsoft.EntityFrameworkCore.Storage;
using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Infrastructure.Data;

namespace TownHubPeople.Infrastructure.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = [];
    private IDbContextTransaction? _transaction;
    private int _transactionDepth;
    private bool _disposed;

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(context);
        }
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);

    // Transactions are reentrant: a nested BeginTransactionAsync joins the outer transaction
    // instead of opening a second one (which relational providers reject). Only the outermost
    // commit actually commits; a rollback at any level aborts the whole transaction.
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            _transaction = await context.Database.BeginTransactionAsync(ct);
        _transactionDepth++;
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            return;

        _transactionDepth--;
        if (_transactionDepth > 0)
            return;

        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
        _transactionDepth = 0;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            context.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
