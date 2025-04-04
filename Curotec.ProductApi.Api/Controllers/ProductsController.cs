using Curotec.ProductApi.Domain.Entities;
using Curotec.ProductApi.Domain.Specifications;
using Curotec.ProductApi.Infrastructure.Caching;
using Microsoft.AspNetCore.Mvc;

namespace Curotec.ProductApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly CachedProductService _cachedService;

    public ProductsController(CachedProductService cachedService)
    {
        _cachedService = cachedService;
    }

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
}