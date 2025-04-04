using Curotec.ProductApi.Application.Interfaces;
using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;
using Curotec.ProductApi.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Curotec.ProductApi.Tests.UnitTests.Caching;

public class CachedProductServiceTests
{
    [Fact]
    public async Task GetCachedProductsAsync_ShouldUseCache()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var mockLogger = new Mock<ILogger<CachedProductService>>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new CachedProductService(mockRepo.Object, memoryCache, mockLogger.Object);

        var spec = new Mock<ISpecification<Product>>().Object;
        var expected = new List<Product> { new Product { Name = "Cached" } };

        memoryCache.Set("products-all-skip0-take0-order-none", expected);

        // Act
        var result = await service.GetCachedProductsAsync(spec);

        // Assert
        Assert.Equal(expected, result);
        mockRepo.Verify(x => x.ListAsync(It.IsAny<ISpecification<Product>>()), Times.Never);
    }
}