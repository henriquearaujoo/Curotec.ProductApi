using Curotec.ProductApi.Api.Controllers;
using Curotec.ProductApi.Application.DTOs;
using Curotec.ProductApi.Application.Interfaces;
using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;
using Curotec.ProductApi.Infrastructure.Caching;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Curotec.ProductApi.Tests.UnitTests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IRepository<Product>> _repoMock;
    private readonly Mock<ICachedProductService> _cacheMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _repoMock = new Mock<IRepository<Product>>();
        _cacheMock = new Mock<ICachedProductService>();
        _controller = new ProductsController(_repoMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnProducts()
    {
        // Arrange
        var products = new List<Product> { new() { Id = 1, Name = "Test", Price = 10 } };
        _cacheMock.Setup(c => c.GetCachedProductsAsync(It.IsAny<ISpecification<Product>>()))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.Get(null, 1, 10, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsAssignableFrom<IReadOnlyList<Product>>(okResult.Value);
        Assert.Single(returnedProducts);
    }

    [Fact]
    public async Task GetById_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test", Price = 10 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, returned.Id);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedProduct()
    {
        var dto = new ProductCreateDto { Name = "New", Price = 12.5m };
        var savedProduct = new Product { Id = 5, Name = "New", Price = 12.5m };

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync(savedProduct);

        var result = await _controller.Create(dto);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        var product = Assert.IsType<Product>(createdAt.Value);
        Assert.Equal("New", product.Name);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        var existing = new Product { Id = 1, Name = "Old", Price = 10 };
        var updateDto = new ProductUpdateDto { Name = "Updated", Price = 15 };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _controller.Update(1, updateDto);

        Assert.IsType<NoContentResult>(result);
        _repoMock.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Name == "Updated")), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        var existing = new Product { Id = 1, Name = "ToDelete", Price = 10 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        _repoMock.Verify(r => r.DeleteAsync(existing), Times.Once);
    }
}