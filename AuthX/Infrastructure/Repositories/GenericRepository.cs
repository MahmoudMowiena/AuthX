using AuthX.Domain.IRepositories;
using AuthX.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AuthXDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AuthXDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task Add(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}