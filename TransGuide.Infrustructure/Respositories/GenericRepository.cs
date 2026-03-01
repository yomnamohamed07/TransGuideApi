using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Data.Repositories;

namespace TransGuide.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly TransGuideDbContext _context;

    public GenericRepository(TransGuideDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(int id) =>
        await _context.Set<T>().FindAsync(id);

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate) =>
        await _context.Set<T>().Where(predicate).ToListAsync();

	public IQueryable<T> GetNoTracking()
	{
		return _context.Set<T>().AsNoTracking().AsQueryable();
	}

	public async Task SaveChangesAsync()
	{
		await _context.SaveChangesAsync();
	}

}