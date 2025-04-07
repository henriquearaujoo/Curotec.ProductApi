using Curotec.ProductApi.Application.Interfaces;
using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;
using Curotec.ProductApi.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Curotec.ProductApi.Tests.UnitTests.Caching;

public class CachedProductServiceTests
{
    private CachedProductService CreateService(
        IRepository<Product> repo,
        IMemoryCache cache,
        ILogger<CachedProductService> logger)
    {
        return new CachedProductService(repo, cache, logger);
    }

    private static ProductWithPaginationSpec BuildSpec(string? search = null)
        => new ProductWithPaginationSpec(search, 1, 10, "nameAsc");

    [Fact]
    public async Task GetCachedProductsAsync_ShouldReturnFromCache_IfAvailable()
    {
        // Arrange
        var spec = BuildSpec();
        var cachedKey = "products-x => ((x.Name.Contains(\"\")))-skip0-take0-order-none";

        var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = new Mock<ILogger<CachedProductService>>().Object;
        var repoMock = new Mock<IRepository<Product>>();
        var products = new List<Product> { new() { Id = 1, Name = "Cached Product" } };

        cache.Set(cachedKey, products); // preload cache manually

        var service = CreateService(repoMock.Object, cache, logger);

        // Act
        var result = await service.GetCachedProductsAsync(spec);

        // Assert
        Assert.Same(products, result);
        repoMock.Verify(r => r.ListAsync(It.IsAny<ISpecification<Product>>()), Times.Never);
    }

    [Fact]
    public async Task GetCachedProductsAsync_ShouldFetchFromRepo_IfNotInCache()
    {
        // Arrange
        var spec = BuildSpec("test");
        var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = new Mock<ILogger<CachedProductService>>().Object;
        var repoMock = new Mock<IRepository<Product>>();

        var products = new List<Product> { new() { Id = 2, Name = "Repo Product" } };
        repoMock.Setup(r => r.ListAsync(spec)).ReturnsAsync(products);

        var service = CreateService(repoMock.Object, cache, logger);

        // Act
        var result = await service.GetCachedProductsAsync(spec);

        // Assert
        Assert.Equal(products, result);
        repoMock.Verify(r => r.ListAsync(spec), Times.Once);
    }

    [Fact]
    public async Task GetCachedProductsAsync_ShouldStoreResultInCache()
    {
        // Arrange
        var spec = BuildSpec("cache");
        var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = new Mock<ILogger<CachedProductService>>().Object;
        var repoMock = new Mock<IRepository<Product>>();
        var products = new List<Product> { new() { Id = 3, Name = "ToCache" } };

        repoMock.Setup(r => r.ListAsync(spec)).ReturnsAsync(products);

        var service = CreateService(repoMock.Object, cache, logger);

        // Act
        await service.GetCachedProductsAsync(spec);
        var cachedKey = "products-x => ((x.Name.Contains(\"cache\")))-skip0-take0-order-none";

        // Assert
        var exists = cache.TryGetValue(cachedKey, out IReadOnlyList<Product>? cachedProducts);
        Assert.True(exists);
        Assert.Equal(products, cachedProducts);
    }
}