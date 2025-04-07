using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;
using Curotec.ProductApi.Infrastructure.Data;
using Curotec.ProductApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Curotec.ProductApi.Tests.UnitTests.Repositories;

public class RepositoryTests
{
    private async Task<(Repository<Product> repository, AppDbContext dbContext)> CreateRepositoryAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        var dbContext = new AppDbContext(options);

        dbContext.Products.AddRange(
            new Product { Name = "Laptop", Price = 1500 },
            new Product { Name = "Mouse", Price = 25 },
            new Product { Name = "Phone", Price = 800 }
        );

        await dbContext.SaveChangesAsync();

        var repository = new Repository<Product>(dbContext);
        return (repository, dbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldInsertProduct()
    {
        var (repo, _) = await CreateRepositoryAsync();
        var product = new Product { Name = "Keyboard", Price = 100 };

        var created = await repo.AddAsync(product);

        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Keyboard", created.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
    {
        var (repo, db) = await CreateRepositoryAsync();
        var product = db.Products.First();

        var found = await repo.GetByIdAsync(product.Id);

        Assert.NotNull(found);
        Assert.Equal(product.Name, found?.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var (repo, _) = await CreateRepositoryAsync();

        var found = await repo.GetByIdAsync(999);

        Assert.Null(found);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnAll_WhenNoCriteria()
    {
        var (repo, _) = await CreateRepositoryAsync();

        var spec = new NoFilterSpecification();
        var result = await repo.ListAsync(spec);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ListAsync_ShouldFilter_ByCriteria()
    {
        var (repo, _) = await CreateRepositoryAsync();

        var spec = new ProductWithPaginationSpec("Phone", 1, 10, null);
        var result = await repo.ListAsync(spec);

        Assert.Single(result);
        Assert.Equal("Phone", result.First().Name);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount_ByCriteria()
    {
        var (repo, _) = await CreateRepositoryAsync();

        var spec = new ProductWithPaginationSpec("Laptop", 1, 10, null);
        var count = await repo.CountAsync(spec);

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyProduct()
    {
        var (repo, db) = await CreateRepositoryAsync();
        var product = db.Products.First();
        product.Name = "Updated Name";

        await repo.UpdateAsync(product);
        var updated = await repo.GetByIdAsync(product.Id);

        Assert.Equal("Updated Name", updated?.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        var (repo, db) = await CreateRepositoryAsync();
        var product = db.Products.First();

        await repo.DeleteAsync(product);
        var deleted = await repo.GetByIdAsync(product.Id);

        Assert.Null(deleted);
    }

    // Dummy spec: no filtering, no sorting, no paging
    private class NoFilterSpecification : ISpecification<Product>
    {
        public Expression<Func<Product, bool>>? Criteria => null;
        public List<Expression<Func<Product, object>>> Includes { get; } = new();
        public Expression<Func<Product, object>>? OrderBy => null;
        public Expression<Func<Product, object>>? OrderByDescending => null;
        public int? Skip => null;
        public int? Take => null;
        public bool IsPagingEnabled => false;
    }
}