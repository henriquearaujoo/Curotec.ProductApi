using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Infrastructure.Data;
using Curotec.ProductApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Curotec.ProductApi.Tests.UnitTests.Repositories;

public class RepositoryTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        using var context = GetInMemoryContext();
        var repo = new Repository<Product>(context);

        var product = new Product { Name = "Test", Price = 9.99M };
        await repo.AddAsync(product);

        Assert.Single(await context.Products.ToListAsync());
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        using var context = GetInMemoryContext();
        var product = new Product { Name = "ToDelete", Price = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repo = new Repository<Product>(context);
        await repo.DeleteAsync(product);

        Assert.Empty(context.Products);
    }
}