using Curotec.ProductApi.Domain.Specifications;

namespace Curotec.ProductApi.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}