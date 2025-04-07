using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;
using Curotec.ProductApi.Infrastructure.Data;
using Curotec.ProductApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Curotec.ProductApi.Tests.UnitTests.Repositories;

public class RepositoryTests
{
    private async Task<(Repository<Product>, AppDbContext)> CreateInMemoryRepo()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        var context = new AppDbContext(options);
        var repo = new Repository<Product>(context);

        // Seed data
        var seedProducts = new List<Product>
        {
            new() { Name = "Phone", Price = 500 },
            new() { Name = "Laptop", Price = 1000 },
            new() { Name = "Mouse", Price = 25 },
        };

        context.Products.AddRange(seedProducts);
        await context.SaveChangesAsync();

        return (repo, context);
    }

    [Fact]
    public async Task AddAsync_ShouldInsertNewEntity()
    {
        var (repo, _) = await CreateInMemoryRepo();
        var product = new Product { Name = "Keyboard", Price = 75 };

        var result = await repo.AddAsync(product);
        var fetched = await repo.GetByIdAsync(result.Id);

        Assert.NotNull(fetched);
        Assert.Equal("Keyboard", fetched.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectEntity()
    {
        var (repo, context) = await CreateInMemoryRepo();
        var existing = context.Products.First();

        var result = await repo.GetByIdAsync(existing.Id);

        Assert.NotNull(result);
        Assert.Equal(existing.Name, result.Name);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnAll_WhenNoSpecification()
    {
        var (repo, _) = await CreateInMemoryRepo();

        var result = await repo.ListAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ListAsync_WithSpecification_ShouldFilterResults()
    {
        var (repo, _) = await CreateInMemoryRepo();
        var spec = new ProductWithPaginationSpec("phone", 1, 10, null);

        var result = await repo.ListAsync(spec);

        Assert.Single(result);
        Assert.Equal("Phone", result.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyEntity()
    {
        var (repo, context) = await CreateInMemoryRepo();
        var product = context.Products.First();
        product.Name = "Updated Name";

        await repo.UpdateAsync(product);
        var updated = await repo.GetByIdAsync(product.Id);

        Assert.Equal("Updated Name", updated.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        var (repo, context) = await CreateInMemoryRepo();
        var product = context.Products.First();

        await repo.DeleteAsync(product);
        var deleted = await repo.GetByIdAsync(product.Id);

        Assert.Null(deleted);
    }
}