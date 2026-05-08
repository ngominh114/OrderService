namespace OrderService.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public virtual async Task<IEnumerable<T>> GetAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default)
        => await Task.FromResult(_dbSet.AsEnumerable().Where(predicate).ToList());

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        => _dbSet.Update(entity);

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        => _dbSet.Remove(entity);

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
