using Curotec.ProductApi.Application.Interfaces;
using Curotec.ProductApi.Domain.Specifications;
using Curotec.ProductApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Curotec.ProductApi.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        var query = SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec);
        return await query.ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        var query = SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec);
        return await query.CountAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}