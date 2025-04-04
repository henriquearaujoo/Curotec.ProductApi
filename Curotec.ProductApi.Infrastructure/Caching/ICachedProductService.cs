using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;

namespace Curotec.ProductApi.Infrastructure.Caching;

public interface ICachedProductService
{
    Task<IReadOnlyList<Product>> GetCachedProductsAsync(ISpecification<Product> spec);
}