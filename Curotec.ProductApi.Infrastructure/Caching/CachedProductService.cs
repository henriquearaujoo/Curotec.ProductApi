using Curotec.ProductApi.Application.Interfaces;
using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Curotec.ProductApi.Infrastructure.Caching;

public class CachedProductService(
    IRepository<Product> repository,
    IMemoryCache cache,
    ILogger<CachedProductService> logger) 
{
    private readonly IRepository<Product> _repository = repository;
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<CachedProductService> _logger = logger;

    private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromMinutes(5); // or inject via options

    public async Task<IReadOnlyList<Product>> GetCachedProductsAsync(ISpecification<Product> spec)
    {
        var cacheKey = GenerateCacheKey(spec);

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Product>? products))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return products!;
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}, querying database...", cacheKey);
        products = await _repository.ListAsync(spec);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _defaultCacheDuration
        };

        _cache.Set(cacheKey, products, cacheOptions);

        return products;
    }

    private static string GenerateCacheKey(ISpecification<Product> spec)
    {
        // Simple cache key using criteria and pagination
        var criteriaPart = spec.Criteria?.ToString() ?? "all";
        var skip = spec.Skip ?? 0;
        var take = spec.Take ?? 0;
        var order = spec.OrderBy?.ToString() ?? spec.OrderByDescending?.ToString() ?? "none";

        return $"products-{criteriaPart}-skip{skip}-take{take}-order-{order}".ToLower();
    }
}