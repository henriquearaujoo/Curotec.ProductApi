using Curotec.ProductApi.Application.DTOs;
using Curotec.ProductApi.Application.Interfaces;
using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;
using Curotec.ProductApi.Infrastructure.Caching;
using Microsoft.AspNetCore.Mvc;

namespace Curotec.ProductApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _repository;
    private readonly CachedProductService _cachedService;

    public ProductsController(IRepository<Product> repository, CachedProductService cachedService)
    {
        _repository = repository;
        _cachedService = cachedService;
    }

    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> Get(
        [FromQuery] string? search,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sort = null)
    {
        var spec = new ProductWithPaginationSpec(search, pageIndex, pageSize, sort);
        var products = await _cachedService.GetCachedProductsAsync(spec);
        return Ok(products);
    }

    // GET /api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product is not null ? Ok(product) : NotFound();
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(ProductCreateDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        };

        var created = await _repository.AddAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

// PUT /api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.Price = dto.Price;

        await _repository.UpdateAsync(existing);
        return NoContent();
    }

    // DELETE /api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        await _repository.DeleteAsync(existing);
        return NoContent();
    }
}