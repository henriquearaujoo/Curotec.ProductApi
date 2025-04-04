using Curotec.ProductApi.Domain.Entities;

namespace Curotec.ProductApi.Domain.Specifications;

public class ProductWithPaginationSpec : BaseSpecification<Product>
{
    public ProductWithPaginationSpec(string? search, int pageIndex, int pageSize, string? sort)
    {
        if (!string.IsNullOrEmpty(search))
            Criteria = p => p.Name.Contains(search);

        ApplyPaging((pageIndex - 1) * pageSize, pageSize);

        OrderBy = sort switch
        {
            "priceAsc" => p => p.Price,
            "priceDesc" => null, 
            _ => p => p.Name
        };

        if (sort == "priceDesc")
            OrderByDescending = p => p.Price;
    }
}